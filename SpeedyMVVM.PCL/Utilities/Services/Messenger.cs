using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public class Messenger : IMessenger
    {
        #region Fileds
        //Dictionary<MessageType, List<Tuple<ValidationToken,MessageAction>>>
        private Dictionary<Type, List<Tuple<WeakReference,WeakAction>>> messagesPool;
        private static object locker = new object();        
        private static volatile Messenger _Default;
        #endregion

        #region Properties
        /// <summary>
        /// Return the default static messanger instance.
        /// </summary>
        public static Messenger Default
        {
            get
            {
                if (_Default == null)
                    ResetDefault();
                return _Default;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Register the call back action with the token passed as parameter. 
        /// </summary>
        /// <typeparam name="T">Type of the action to be registered.</typeparam>
        /// <param name="action">Call back action to be registered.</param>
        /// <param name="validationToken">Token to be validate while propagating a message.</param>
        public void Register<Tmessage>(Action<Tmessage> callBack, object validationToken = null)
        {
            if (callBack == null)
                throw new ArgumentNullException("Action required!");
            lock (locker)
            {
                List<Tuple<WeakReference, WeakAction>> target;
                if (!messagesPool.TryGetValue(typeof(Tmessage), out target))
                {
                    target = new List<Tuple<WeakReference, WeakAction>>()
                    { Tuple.Create(new WeakReference(validationToken), new WeakAction(callBack)) };
                    messagesPool.Add(typeof(Tmessage), target);
                    return;
                }
                var act = target.Where(t => t.Item2.Instance.Target == validationToken).FirstOrDefault();

                if (act.IsDefault())
                    target.Add(Tuple.Create(new WeakReference(validationToken), new WeakAction(callBack)));
                else
                {
                    act = Tuple.Create(new WeakReference(validationToken), new WeakAction(callBack));
                }
            }
        }

        /// <summary>
        /// Remove the registered call back action and the related validation token reference.
        /// </summary>
        /// <typeparam name="T">Type of the action to be removed.</typeparam>
        /// <param name="callBack">Call back action to be removed.</param>
        public void UnRegister<Tmessage>(Action<Tmessage> callBack)
        {
            if (callBack == null)
                throw new ArgumentNullException("Action required!");
            lock (locker)
            {
                var tupleToRemove = messagesPool.Values.SelectMany(t => t.Where(r => r.Item2.MethodType == callBack.GetType() && 
                                                                                     r.Item2.Instance.Target == callBack.Target));
                foreach (var tuple in tupleToRemove)
                    messagesPool.Where(k => k.Value.Contains(tuple))
                                         .Select(k => k.Value.Remove(tuple));
            };
            CleanUp();
        }

        /// <summary>
        /// Remove all the call back actions registered with the token passed as parameter.
        /// </summary>
        /// <param name="validationToken">Recipient to be removed.</param>
        public void UnRegister(object validationToken)
        {
            lock (locker)
            {
                var tupleToRemove = messagesPool.Values.SelectMany(t => t.Where(r => r.Item1.Target == validationToken));
                foreach (var tuple in tupleToRemove)
                    messagesPool.Where(k => k.Value.Contains(tuple))
                                         .Select(k => k.Value.Remove(tuple));
            }
            CleanUp();
        }
        
        /// <summary>
        /// Send a message to the broker to execute the registered actions for the parameter.
        /// </summary>
        /// <typeparam name="Tmessage">Type of the message to be send.</typeparam>
        /// <param name="sender">Sender of the message.</param>
        /// <param name="message">Message to be send.</param>
        /// <param name="notifySender">Set true in case we want to execute also the call back action of the sender.</param>
        /// <param name="tokenValidationRule">Function to check if the receiver can recive the message using the receiver validationToken.</param>
        /// <returns>Return true when the message have been send correctly.</returns>
        public bool Send<Tmessage>(object sender, Tmessage message, bool notifySender = false, Func<Tmessage, object, bool> tokenValidationRule = null)
        {
            CleanUp();
            lock (locker)
            {
                List<Delegate> delegates;
                if (tokenValidationRule != null)
                    delegates = messagesPool[typeof(Tmessage)].Where(t => notifySender ? true : t.Item2.Instance.Target != sender)
                                                              .Where(t => tokenValidationRule(message, t.Item1.Target))
                                                              .Select(t => t.Item2.CreateDelegate()).ToList();
                else
                    delegates = messagesPool[typeof(Tmessage)].Where(t=> notifySender ? true : t.Item2.Instance.Target != sender)
                                                              .Select(t => t.Item2.CreateDelegate()).ToList();
                
                if (delegates != null && delegates.Count() > 0)
                {
                    Delegate.Combine(delegates.ToArray()).DynamicInvoke(message);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Reset the static instance of messenger (create a new instance).
        /// </summary>
        public static void ResetDefault()
        {
            lock (locker)
            {
                _Default = new Messenger();
            }
        }

        /// <summary>
        /// Remove all the disposed objects from the current instance.
        /// </summary>
        public void CleanUp()
        {
            //Remove all the keys which haven't got any tuple. 
            messagesPool.Where(k => k.Value.Count == 0)
                    .Select(k => messagesPool.Remove(k.Key));
            //Remove all the references to disposed objects.
            messagesPool.Where(kv => kv.Value.Any(t => !t.Item1.IsAlive || !t.Item2.Instance.IsAlive))
                    .Select(kv => messagesPool[kv.Key].RemoveAll(t => !t.Item1.IsAlive || !t.Item2.Instance.IsAlive));
        }
        #endregion

        #region Constructors/Distructors
        public Messenger()
        {
            messagesPool = new Dictionary<Type, List<Tuple<WeakReference, WeakAction>>>();
        }
        ~Messenger()
        {
            messagesPool.Clear();
            messagesPool = null;
        }
        #endregion
    }
}
