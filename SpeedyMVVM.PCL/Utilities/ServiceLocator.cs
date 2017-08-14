using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Class to provide a container where inject and retrive services for the application.
    /// </summary>
    public class ServiceLocator : IServiceProvider
    {

        #region Fields
        private Dictionary<Type, object> services;
        #endregion

        #region Costructors
        /// <summary>
        /// Create a new instance of ServiceLocator.
        /// </summary>
        public ServiceLocator()
        {
            services = new Dictionary<Type, object>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Static function to return a new instance of ServiceLocator.
        /// </summary>
        public static ServiceLocator Instance { get { return new ServiceLocator(); } }
        #endregion

        #region Methods
        /// <summary>
        /// Remove te service from the dictionary.
        /// </summary>
        /// <typeparam name="T">Type of service to remove.</typeparam>
        public void RemoveService<T>()
        {
            lock (services)
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
            lock (services)
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
            object service = new object();
            lock (services)
            {
                services.TryGetValue(typeof(T), out service);
            }
            return (T)service;
        }

        /// <summary>
        /// Get a registered service.
        /// </summary>
        /// <param name="serviceType">Type of the service to retrive.</param>
        /// <returns>Returned service.</returns>
        public object GetService(Type serviceType)
        {
            lock (services)
            {
                if (services.ContainsKey(serviceType))
                {
                    return services[serviceType];
                }
            }
            return null;
        }
        #endregion
    }
}
