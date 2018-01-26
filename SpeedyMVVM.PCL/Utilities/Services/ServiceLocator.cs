using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Class to provide a container where inject and retrive services for the application.
    /// </summary>
    public class ServiceLocator : IServiceProvider, IDisposable
    {

        #region Fields
        private Dictionary<Type, object> services = new Dictionary<Type, object>();
        private object locker = new object();
        private static volatile ServiceLocator _Default;
        private static object syncLock = new object();
        /// <summary>
        /// Static field to return a default instance of ServiceLocator.
        /// </summary>
        public static ServiceLocator Default
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
        /// Remove te service from the dictionary.
        /// </summary>
        /// <typeparam name="T">Type of service to remove.</typeparam>
        public void RemoveService<T>()
        {
            lock (locker)
            {
                services.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Add a new service into the dictionary. By default, in case will exist already into the Dictionary, it will be overrided.
        /// </summary>
        /// <typeparam name="T">Type of service to add.</typeparam>
        /// <param name="service">Instance of the service to add.</param>
        /// <returns>Return 'TRUE' if could add the service to the dictionary.</returns>
        public bool RegisterService<T>(T service)
        {
            return RegisterService<T>(service, true);
        }

        /// <summary>
        /// Add a new service into the dictionary.
        /// </summary>
        /// <typeparam name="T">Type of service to add.</typeparam>
        /// <param name="service">Instance of the service to add.</param>
        /// <param name="overwriteIfExists">If 'TRUE' will override in case the service is already into the dictionary.</param>
        /// <returns>Return 'TRUE' if could add the service to the dictionary.</returns>
        public bool RegisterService<T>(T service, bool overwriteIfExists)
        {
            if (service == null)
                throw new ArgumentNullException("Service required!");
            lock (locker)
            {
                if (!services.ContainsKey(typeof(T)))
                {
                    services.Add(typeof(T), service);
                    return true;
                }
                else if (overwriteIfExists)
                {
                    services[typeof(T)] = service;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to retrive.</typeparam>
        /// <returns>Returned service.</returns>
        public T GetService<T>()
        {
            object service;
            lock (locker)
            {
                services.TryGetValue(typeof(T), out service);
            }
            return (service != null) ? (T)service : default(T);
        }

        /// <summary>
        /// Get a registered service.
        /// </summary>
        /// <param name="serviceType">Type of the service to retrive.</param>
        /// <returns>Returned service.</returns>
        public object GetService(Type serviceType)
        {
            lock (locker)
            {
                if (services.ContainsKey(serviceType))
                {
                    return services[serviceType];
                }
            }
            return null;
        }

        /// <summary>
        /// Empty the ServiceContainer.
        /// </summary>
        public void Clear()
        {
            lock (locker)
            {
                services.Clear();
            }
        }

        public static void ResetDefault()
        {
            lock (syncLock)
            {
                _Default = new ServiceLocator();
            }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Dispose all the IDisposable Services contained into the service container, than dispose the instance of ServiceContainer.
        /// </summary>
        public void Dispose()
        {
            lock (locker)
            {
                var disposableServices = services.Where(s => s.Value is IDisposable).Select(s => s.Value).Cast<IDisposable>();
                foreach (var service in disposableServices)
                    service.Dispose();
                services.Clear();
                services = null;
            }
            locker = null;
        }
        #endregion
    }
}
