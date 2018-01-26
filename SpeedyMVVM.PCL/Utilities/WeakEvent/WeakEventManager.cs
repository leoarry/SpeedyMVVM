using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public class WeakEventManager
    {
        #region Fields
        private readonly ConcurrentDictionary<WeakReference, List<WeakEvent>> eventHandlers = new ConcurrentDictionary<WeakReference, List<WeakEvent>>();
        private bool _CleaningInProgress = false;
        private static volatile WeakEventManager _Default;
        private static object syncRoot = new Object();
        //private Timer cleaningTimer;
        #endregion

        #region Properties
        /// <summary>
        /// Return a value telling if the cleaning of disposed reference is performing.
        /// </summary>
        public bool CleaningInProgress { get { return _CleaningInProgress; } }

        /// <summary>
        /// Static default instance of WeakEventManager.
        /// </summary>
        public static WeakEventManager Default
        {
            get
            {
                if (_Default == null)
                    ResetDefault();
                return _Default;
            }
        }
        #endregion

        #region Add Handler Method Overloads
        /// <summary>
        /// Add the handler to all the sources of type <typeparamref name="T"/> registered with the event <paramref name="eventName"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T>(string eventName, EventHandler handler)
        {
            CreateWakeEvent(default(T), eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Add the handler to all the sources of type <typeparamref name="T"/> registered with the event <paramref name="eventName"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T, TEventArgs>(string eventName, EventHandler<TEventArgs> handler)
        {
            CreateWakeEvent(default(T), eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T>(T source, string eventName, EventHandler handler)
        {
            CreateWakeEvent(source, eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T, TEventArgs>(T source, string eventName, EventHandler<TEventArgs> handler)
        {
            CreateWakeEvent(source, eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T>(T source, string eventName, PropertyChangedEventHandler handler) where T:INotifyPropertyChanged
        {
            CreateWakeEvent(source, eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void AddEventHandler<T>(T source, string eventName, NotifyCollectionChangedEventHandler handler) where T:INotifyCollectionChanged
        {
            CreateWakeEvent(source, eventName, handler.Target, handler.GetMethodInfo());
        }
        #endregion

        #region Remove Handler Method Overloads
        /// <summary>
        /// Remove a specific source and all the relative events and listeners.
        /// </summary>
        /// <typeparam name="T">Type of the source to be removed.</typeparam>
        /// <param name="source">Source to be removed.</param>
        public void RemoveSource(object source)
        {
            List<WeakEvent> removedEvents;
            var sources = eventHandlers.Where(kv => Equals(kv.Key.Target, source))
                                      .Select(kv => kv.Key);
            foreach (var k in sources)
                eventHandlers.TryRemove(k, out removedEvents);
        }

        /// <summary>
        /// Remove a specific listener, going throw all the events of all the sources.
        /// </summary>
        /// <param name="listener">Listener to be removed.</param>
        public void RemoveListener(object listener)
        {
            RemoveWakeEvent<object>(null, null, listener);
        }

        /// <summary>
        /// Remove a specific listener associated to a specific event, going throw all the sources.
        /// </summary>
        /// <typeparam name="T">Type of the source.</typeparam>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">Instance of the listener to be removed.</param>
        public void RemoveListener<T>(object listener, string eventName)
        {
            RemoveWakeEvent<T>(default(T), eventName, listener);
        }

        /// <summary>
        /// Remove the listener from the source <paramref name="source"/> associated to the event <paramref name="eventName"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source.</typeparam>
        /// <param name="source">Source of the event.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">Instance of the listener to be removed.</param>
        public void RemoveEventHandler<T>(T source, string eventName, EventHandler handler)
        {
            RemoveWakeEvent(source, eventName, handler.Target);
        }

        /// <summary>
        /// Remove the listener from the source <paramref name="source"/> associated to the event <paramref name="eventName"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source.</typeparam>
        /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
        /// <param name="source">Source of the event.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">Instance of the listener to be removed.</param>
        public void RemoveEventHandler<T, TEventArgs>(T source, string eventName, EventHandler<TEventArgs> handler)
        {
            RemoveWakeEvent(source, eventName, handler.Target);
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void RemoveEventHandler<T>(T source, string eventName, PropertyChangedEventHandler handler) where T : INotifyPropertyChanged
        {
            RemoveWakeEvent(source, eventName, handler.Target);
        }

        /// <summary>
        /// Add the handler to the source.
        /// </summary>
        /// <typeparam name="T">Type of the source of the event.</typeparam>
        /// <param name="source">Source of the event to reference.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">Handler of the event.</param>
        public void RemoveEventHandler<T>(T source, string eventName, NotifyCollectionChangedEventHandler handler) where T : INotifyCollectionChanged
        {
            RemoveWakeEvent(source, eventName, handler.Target);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset the default static instance of WeakEventManager.
        /// </summary>
        public static void ResetDefault()
        {
            lock (syncRoot)
            {
                _Default = new WeakEventManager();
            }
        }

        /// <summary>
        /// Raise the event <paramref name="eventName"/>.
        /// </summary>
        /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
        /// <param name="sender">Source of the event.</param>
        /// <param name="args">Argouments for the event.</param>
        /// <param name="eventName">Name of the event.</param>
        public void RaiseEvent<TEventArgs>(object sender, TEventArgs args, string eventName)
        {
            var res = eventHandlers.Where(kv => kv.Key.IsAlive && kv.Key.Target == sender)
                         .SelectMany(kv => kv.Value.Where(we => we.EventName == eventName));
            foreach (var we in res)
                we.RaiseEvent(sender, args);
        }

        /// <summary>
        /// Clean up all the disposed references.
        /// </summary>
        /// <returns></returns>
        public Task CleanUp()
        {
            return Task.Factory.StartNew(() => 
            {
                _CleaningInProgress = true;
                //Clean sources without events or disposed 
                var disposedSources = eventHandlers.Where(kv => !kv.Key.IsAlive || kv.Value.IsNullOrEmpty());
                foreach(var source in disposedSources)
                {
                    List<WeakEvent> events;
                    eventHandlers.TryRemove(source.Key, out events);
                }
                //Clean disposed listeners
                var disposedEvents = eventHandlers.SelectMany(kv => kv.Value);
                foreach (var e in disposedEvents)
                    e.CleanUp();

                _CleaningInProgress = false;
            });
        }
        
        /// <summary>
        /// Return a value telling whatever the listener is contains into the WeakEventManager.
        /// </summary>
        /// <param name="listener">Listener to look for.</param>
        /// <returns></returns>
        public bool ContainsListener(object listener)
        {
            return eventHandlers.Any(kv => kv.Value.Any(we => we.ContainsListener(listener)));
        }

        /// <summary>
        /// Return a value telling whatever the listener is contains into the WeakEventManager.
        /// </summary>
        /// <typeparam name="T">Type of the source.</typeparam>
        /// <param name="source">Source to look for.</param>
        /// <returns></returns>
        public bool ContainsSource<T>(T source)
        {
            return eventHandlers.Any(kv => Equals(kv.Key.Target, source));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Create a weak event. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        /// <param name="method"></param>
        private void CreateWakeEvent<T>(T source, string eventName, object listener, MethodInfo method)
        {
            var sourceKey = eventHandlers.Where(kp => object.Equals(kp.Key.Target, source)).FirstOrDefault();
            IEnumerable<WeakEvent> events;

            //If the source is null retrieve all the weak events careless of the source and add the handler
            if (source.IsDefault())
                events = eventHandlers.Where(kv => kv.Key.Target.GetType().Equals(typeof(T)))
                                     .SelectMany(kv => kv.Value.Where(we => we.EventName == eventName));
            else if (sourceKey.IsDefault())
            {
                eventHandlers.TryAdd(new WeakReference(source), new List<WeakEvent> { new WeakEvent(eventName, listener, method) });
                return;
            }
            else if (sourceKey.Value.Where(we => we.EventName == eventName).Count() == 0)
            {
                eventHandlers[sourceKey.Key].Add(new WeakEvent(eventName, listener, method));
                return;
            }
            else
                events = eventHandlers[sourceKey.Key].Where(we => we.EventName == eventName);

            foreach (var e in events)
                e.AddListener(listener, method);
        }

        /// <summary>
        /// Remove a weak event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        private void RemoveWakeEvent<T>(T source, string eventName, object listener)
        {
            IEnumerable<WeakEvent> events = null;

            if (!source.IsDefault() && !string.IsNullOrEmpty(eventName) && listener != null)
                events = eventHandlers.Where(kv => Equals(kv.Key.Target, source))
                                      .SelectMany(kv => kv.Value.Where(we => we.EventName == eventName && we.ContainsListener(listener)));
            else if (source.IsDefault() && string.IsNullOrEmpty(eventName) && listener != null)
                events = eventHandlers.SelectMany(kv => kv.Value.Where(we => we.ContainsListener(listener)));
            else if (source.IsDefault() && !string.IsNullOrEmpty(eventName) && listener != null)
                events = eventHandlers.SelectMany(kv => kv.Value.Where(we => we.EventName == eventName && we.ContainsListener(listener)));

            foreach (var e in events)
                e.RemoveListener(listener);
        }
        #endregion
    }
}
