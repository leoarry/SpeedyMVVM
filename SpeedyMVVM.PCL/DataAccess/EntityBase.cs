using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Basic class to build an Entity for data-access purpose.
    /// </summary>
    public abstract class EntityBase : ObservableObject, IEntityBase
    {
        #region Fields
        private EntityStatusEnum _EntityStatus;
        private int _ID;
        #endregion

        #region Properties
        /// <summary>
        /// ID of the Entity
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set
            {
                if (value != _ID)
                {
                    _ID = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

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
        /// Raises this object's PropertyChanged event and set the EntityStatus as Modified.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            this.EntityStatus = EntityStatusEnum.Modified;
            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
