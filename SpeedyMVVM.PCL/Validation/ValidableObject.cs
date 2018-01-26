using SpeedyMVVM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    /// <summary>
    /// Base implementation of INotifyDataErrorInfo and IValidator to validate the properties.
    /// </summary>
    [DataContract]
    public abstract class ValidableObject : ObservableObject, INotifyDataErrorInfo
    {
        #region Properties
        /// <summary>
        /// Validation service to validate the object properties.
        /// </summary>
        [DataMember]
        public IValidator Validator { get; set; }

        /// <summary>
        /// The object is valid, no errors occurs.
        /// </summary>
        [DataMember]
        public bool IsValid
        {
            get { return !HasErrors; }
        }

        /// <summary>
        /// The object has errors, is not valid.
        /// </summary>
        [DataMember]
        public bool HasErrors
        {
            get { return (Validator != null) ? Validator.HasErrors : false; }
        }
        #endregion

        #region Events and Delegates
        /// <summary>
        /// Raised when one error occur.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Get the error messages for the property passed as parameter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Collection of error messages.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return (Validator != null) ? Validator.GetErrors(propertyName) : null;
        }

        /// <summary>
        /// Executed when a value of a property change. Will validate the property.
        /// </summary>
        /// <param name="propertyName">Name of the property </param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (Validator != null && propertyName != nameof(HasErrors))
                Validator.ValidateProperty(this, propertyName);
            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Executed when an error occur
        /// </summary>
        /// <param name="propertyName">Name of the property having errors.</param>
        public virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
        #endregion
    }
}
