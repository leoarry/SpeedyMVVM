using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.ComponentModel;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Basic class to build an Entity for data-access purpose.
    /// </summary>
    public abstract class EntityBase : ObservableObject
    {
        #region Fields
        private EntityStatusEnum _EntityStatus;
        private int _ID;
        #endregion
        
        #region Properties
        /// <summary>
        /// Actual Status of the Entity.
        /// </summary>
        public EntityStatusEnum EntityStatus
        {
            get { return _EntityStatus; }
            set
            {
                if (_EntityStatus != value)
                {
                    _EntityStatus = value;
                    OnPropertyChanged(nameof(EntityStatus));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Make a memberwise clone of the current instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Clone<T>() where T : EntityBase
        {
            return (T)this.MemberwiseClone();
        }
        
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if(propertyName!=nameof(EntityStatus))
                this.EntityStatus = EntityStatusEnum.Modified;
            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
