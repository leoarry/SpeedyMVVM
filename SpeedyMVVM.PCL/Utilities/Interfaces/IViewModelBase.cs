using System;
using System.ComponentModel;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Define a basic view model.
    /// </summary>
    public interface IViewModelBase: INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Reference to the application service locator.
        /// </summary>
        ServiceLocator ServiceContainer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the view model services have been injected.
        /// </summary>
        bool IsInitialized { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the ViewModel injecting the services from the ServiceLocator passed as parameter.
        /// </summary>
        /// <param name="locator">Service locator for service injection</param>
        void InjectServices(ServiceLocator locator);

        /// <summary>
        /// Get a service from the 'ServiceContainer'.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>Requested service.</returns>
        T GetService<T>();
        #endregion
    }
}
