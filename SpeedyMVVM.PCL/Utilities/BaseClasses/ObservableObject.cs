using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Abstract class which implement the INotifyPropertyChanged
    /// </summary>
    [DataContract]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                WeakEventManager.Default.AddEventHandler(this, nameof(PropertyChanged), value);
            }
            remove
            {
                WeakEventManager.Default.RemoveEventHandler(this, nameof(PropertyChanged), value);
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) 
                || this.GetType().GetRuntimeProperty(propertyName) != null,
                "Couldn't find the property for this instance");
            var e = new PropertyChangedEventArgs(propertyName);
            WeakEventManager.Default.RaiseEvent(this, e, nameof(PropertyChanged));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyNames"></param>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");
            foreach (string p in propertyNames)
                OnPropertyChanged(p);
        }
        #endregion

        #region SetProperty Methods
        /// <summary>
        /// Set the property <paramref name="propertyName"/> and raise PropertyChanged if the current value is different than the new value.
        /// </summary>
        /// <typeparam name="TProp">Type of the property.</typeparam>
        /// <param name="currentValue">Current value of the property.</param>
        /// <param name="newValue">New value of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetProperty<TProp>( ref TProp currentValue, TProp newValue, [CallerMemberName] string propertyName = null)
        {
            if (!object.Equals(currentValue, newValue))
            {
                currentValue = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the property <paramref name="propertyNames"/> and raise PropertyChanged if the current value is different than the new value.
        /// </summary>
        /// <typeparam name="TProp">Type of the property.</typeparam>
        /// <param name="currentValue">Current value of the property.</param>
        /// <param name="newValue">New value of the property.</param>
        /// <param name="propertyNames">Name of the property.</param>
        /// <returns></returns>
        protected bool SetProperty<TProp>( ref TProp currentValue, TProp newValue, params string[] propertyNames)
        {
            if (!object.Equals(currentValue, newValue))
            {
                currentValue = newValue;
                OnPropertyChanged(propertyNames);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the property <paramref name="propertyName"/> and raise PropertyChanged if the current value is different than the new value.
        /// </summary>
        /// <param name="equal"></param>
        /// <param name="action"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty(Func<bool> equal, Action action, [CallerMemberName] string propertyName = null)
        {
            if (equal())
                return false;
            action();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Set the property <paramref name="propertyNames"/> and raise PropertyChanged if the current value is different than the new value.
        /// </summary>
        /// <param name="equal"></param>
        /// <param name="action"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        protected bool SetProperty(Func<bool> equal, Action action, params string[] propertyNames)
        {
            if (equal())
                return false;
            action();
            OnPropertyChanged(propertyNames);
            return true;
        }
        #endregion

        #region Contructors/Destructors

        ~ObservableObject()
        {
            WeakEventManager.Default.RemoveSource(this);
        }
        #endregion
    }
}
