using System;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Provide a basic class for ViewModel  implementation.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, IViewModelBase
    {
        #region Fields
        private ServiceLocator _ServiceContainer;
        private bool _IsInitialized;
        #endregion

        #region Properties
        /// <summary>
        /// Reference to the application service locator.
        /// </summary>
        public ServiceLocator ServiceContainer
        {
            get { return this._ServiceContainer; }
            set { _ServiceContainer = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the view model services have been injected.
        /// </summary>
        public bool IsInitialized
        {
            get { return _IsInitialized; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get a service from the 'ServiceContainer'.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>Requested service or the default instance in case service container is null.</returns>
        public T GetService<T>()
        {
            if (_ServiceContainer != null)
                return _ServiceContainer.GetService<T>();
            else
                return default(T);
        }

        /// <summary>
        /// Initialize the ViewModel injecting the services from the ServiceLocator passed as parameter.
        /// </summary>
        /// <param name="locator">Service locator for service injection</param>
        public virtual void InjectServices(ServiceLocator locator)
        {
            _ServiceContainer = locator;
            _IsInitialized = true;
        }
        #endregion
    }
}
