using SpeedyMVVM.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    public static class ValidationExtentionMethods
    {
        #region Validable Object Extention Methods

        /// <summary>
        /// Get the IValidator of the current instance, creating new Validator in case IValidator property is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static AsyncValidator<T> GetAsyncValidator<T>(this T instance) where T : ValidableObject
        {
            if (instance.Validator == null)
                instance.Validator = new AsyncValidator<T>();
            return (AsyncValidator<T>)instance.Validator;
        }

        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will add new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public static void Validate<T>(this T instance) where T : ValidableObject
        {
            instance.Validator.Validate(instance).Wait();
        }

        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will add new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public static void ValidateProperty<T>(this T instance, string propertyName) where T : ValidableObject
        {
            instance.Validator.ValidateProperty(instance, propertyName).Wait();
        }

        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will add new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public static Task ValidateAsync<T>(this T instance) where T : ValidableObject
        {
            return instance.Validator.Validate(instance);
        }

        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will add new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public static Task ValidatePropertyAsync<T>(this T instance, string propertyName) where T : ValidableObject
        {
            return instance.Validator.ValidateProperty(instance, propertyName);
        }
        #endregion

        #region Validation Rule Collection Extention Methods
        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will add new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public static ValidationRule<T> Property<T>(this List<ValidationRule<T>> instance, Expression<Func<T, object>> property) where T : ValidableObject
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Can't get the desired property, instance is null.");
            var name = property.GetFullMemberName();
            if (instance.Where(v => v.PropertyName == name && Type.Equals(v.MemberType, typeof(T))).Count() == 0)
                instance.Add(new ValidationRule<T>(property));
            return instance.Where(v => v.PropertyName == name && Type.Equals(v.MemberType, typeof(T)))
                           .FirstOrDefault();
        }

        /// <summary>
        /// Add a new rule action to the property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="property">Property to validate</param>
        /// <param name="action">Action to perform</param>
        /// <param name="message">Error message.</param>
        public static void AddRule<T>(this List<ValidationRule<T>> instance, Expression<Func<T,object>> property, Func<T,bool> action, string message="") where T : ValidableObject
        {
            instance.Property(property).AddRuleAction(action, message);
        }
        #endregion
    }
    
}
