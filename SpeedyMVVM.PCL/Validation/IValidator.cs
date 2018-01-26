using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    /// <summary>
    /// Generic wrapping for IValidator interface.
    /// </summary>
    /// <typeparam name="T">Type of ValidableObject.</typeparam>
    public interface IValidator<T> : IValidator where T : ValidableObject
    {
        /// <summary>
        /// Collection of rules for the validation.
        /// </summary>
        ConcurrentBag<ValidationRule<T>> RulesCollection { get; set; }

        /// <summary>
        /// Get the validation rule object for the selected property
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="property">Property</param>
        /// <returns></returns>
        ValidationRule<T> Property(Expression<Func<T, object>> property);

        /// <summary>
        /// Merge the rules on the list passed as parameter with the current rules.
        /// </summary>
        /// <typeparam name="T">Type of the validable object.</typeparam>
        /// <param name="instance">Validable Object.</param>
        /// <param name="newRules">Rules to merge.</param>
        /// <param name="mergingAction">How to merge the collections.</param>
        /// <returns></returns>
        ConcurrentBag<ValidationRule<T>> MergeRules(ConcurrentBag<ValidationRule<T>> newRules, MergingActionEnum mergingAction = MergingActionEnum.ReplaceRules);
    }

    /// <summary>
    /// Interface to manage the data errors notifications.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Get a list of errors for the property passed as parameter.
        /// </summary>
        /// <param name="propertyName">Name of the property to retrieve errors.</param>
        /// <returns>List of error messages.</returns>
        IEnumerable GetErrors(string propertyName);

        /// <summary>
        /// Check if the instance have any errors.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Validate the property passed as parameter.
        /// </summary>
        /// <param name="instance">Type of the object.</param>
        /// <param name="propertyName">Name of the property to be validated.</param>
        /// <returns></returns>
        Task ValidateProperty(object instance, string propertyName);

        /// <summary>
        /// Validate all the properties of the object to be validated.
        /// </summary> 
        /// <param name="instance">Type of the object.</param>
        /// <returns></returns>
        Task Validate(object instance);
    }
}
