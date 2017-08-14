using System;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.Utilities.Interfaces;

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
            set
            {
                if (value != _ServiceContainer)
                {
                    _ServiceContainer = value;
                    OnPropertyChanged(nameof(ServiceContainer));
                }
            }
        }

        /// <summary>
        /// 'TRUE' when the ViwModel is initialized
        /// </summary>
        public bool IsInitialized
        {
            get { return _IsInitialized; }
            set
            {
                if (_IsInitialized != value)
                {
                    _IsInitialized = value;
                    OnPropertyChanged(nameof(IsInitialized));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get a service from the 'ServiceContainer'.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>Requested service.</returns>
        public T GetService<T>()
        {
            return this._ServiceContainer.GetService<T>();
        }

        /// <summary>
        /// Initialize the ViewModel
        /// </summary>
        /// <param name="locator">Service locator for service injection</param>
        public abstract void Initialize(ServiceLocator locator);
        
        #endregion

    }
}
