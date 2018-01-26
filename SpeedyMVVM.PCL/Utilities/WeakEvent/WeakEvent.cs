using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    internal class WeakEvent
    {
        #region Properties and Fields
        private object locker = new object();
        private List<WeakAction> listeners;

        /// <summary>
        /// Name of the event.
        /// </summary>
        public readonly string EventName;
        #endregion

        #region Public Methods
        /// <summary>
        /// Return a value saying if the listener passed as parameter is present into the collection.
        /// </summary>
        /// <param name="listener">Listener to check for.</param>
        /// <returns></returns>
        public bool ContainsListener(object listener)
        {
            lock(locker)
                return listeners.Any(t => t.Instance.Target == listener);
        }

        /// <summary>
        /// Remove all the disposed listeners from this event.
        /// </summary>
        /// <returns></returns>
        public int CleanUp()
        {
            lock (locker)
                return listeners.RemoveAll(t => !t.Instance.IsAlive);
        }

        /// <summary>
        /// Add a new listener (in case don't exist) to the event.
        /// </summary>
        /// <param name="listener">listener to be registered with the event.</param>
        /// <param name="method">handler for the event.</param>
        /// <returns>return true when can add a new reference.</returns>
        public bool AddListener(object listener, MethodInfo method)
        {
            lock (locker)
            {
                if (!ContainsListener(listener))
                {
                    listeners.Add(new WeakAction(listener, method));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove the listener from the event and clean up the disposed references.
        /// </summary>
        /// <param name="listener">listener to be removed.</param>
        /// <returns></returns>
        public int RemoveListener(object listener)
        {
            lock (locker)
            {
                return listeners.RemoveAll(t => t.Instance.Target == listener || !t.Instance.IsAlive);
            }
        }

        /// <summary>
        /// Raise the event.
        /// </summary>
        /// <typeparam name="TEventArgs">Type of the event.</typeparam>
        /// <param name="sender">Source of the event.</param>
        /// <param name="args">Argouments of the event.</param>
        /// <returns></returns>
        public bool RaiseEvent<TEventArgs>(object sender, TEventArgs args)
        {
            lock (locker)
            {
                if (listeners.Count > 0)
                {
                    var delegates = listeners.Where(t => t.Instance.IsAlive)
                                             .Select(t => t.CreateDelegate());
                    if(!delegates.IsNullOrEmpty())
                        Delegate.Combine(delegates.ToArray()).DynamicInvoke(sender, args);
                }
                CleanUp();
                return true;
            }
        }
        #endregion

        #region Contructors/Distructors
        /// <summary>
        /// Create a new instance of WeakEvent.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public WeakEvent(string eventName)
        {
            EventName = eventName;
            listeners = new List<WeakAction>();
        }

        /// <summary>
        /// Create a new instance of WeakEvent.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">Listener to be registered.</param>
        /// <param name="method">Handler for the event.</param>
        public WeakEvent(string eventName, object listener, MethodInfo method)
        {
            EventName = eventName;
            listeners = new List<WeakAction>();
            listeners.Add(new WeakAction(listener, method));
        }

        ~WeakEvent()
        {
            listeners.Clear();
            listeners = null;
        }
        #endregion
    }
}
