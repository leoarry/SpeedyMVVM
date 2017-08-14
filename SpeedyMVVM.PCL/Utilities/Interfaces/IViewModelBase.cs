using System.ComponentModel;

namespace SpeedyMVVM.Utilities.Interfaces
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
        /// 'TRUE' when the ViwModel is initialized
        /// </summary>
        bool IsInitialized { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        /// <param name="locator">Service locator for service injection</param>
        void Initialize(ServiceLocator locator);
        /// <summary>
        /// Get a service from the 'ServiceContainer'.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>Requested service.</returns>
        T GetService<T>();
        #endregion
    }
}
