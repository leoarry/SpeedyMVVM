using SpeedyMVVM.Utilities;
using SpeedyMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;
using SpeedyMVVM.Expressions;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace SpeedyMVVM.Validation
{
    /// <summary>
    /// Validator class to manage the data errors notifications using the async pattern. Implementation of IValidator.
    /// </summary>
    public class AsyncValidator<T> : IValidator<T> where T : ValidableObject
    {
        #region Fields
        private object locker = new object();
        private Dictionary<string,List<string>> errors;
        #endregion

        #region Properties
        /// <summary>
        /// Return true when there are not errors.
        /// </summary>
        public bool IsValid { get { return !HasErrors; } }

        /// <summary>
        /// Return true when there are errors.
        /// </summary>
        public bool HasErrors { get { return errors.Count > 0; } }

        /// <summary>
        /// Collection of rules for the validation.
        /// </summary>
        public ConcurrentBag<ValidationRule<T>> RulesCollection { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Get the error messages of the actual rules with errors.
        /// </summary>
        /// <param name="propertyName">Name of the property to look for.</param>
        /// <returns>Return a collection of error messages.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return new List<string>();
            lock (locker)
            {
                return errors.Where(e=> e.Key==propertyName)
                         .Select(e=> e.Value).FirstOrDefault() ?? new List<string>();
            }
        }

        /// <summary>
        /// Validate all the properties included in Rules.
        /// </summary>
        /// <returns></returns>
        public Task Validate(object instance)
        {
            return Task.Factory.StartNew(async() =>
            {
                if (!Type.Equals(instance.GetType(), typeof(T)))
                    return;
                var rules = RulesCollection.Select(r => r.PropertyName);
                foreach (var pName in rules)
                    await ValidateProperty(instance, pName);
            });
        }

        /// <summary>
        /// Validate a specific property.
        /// </summary>
        /// <param name="propertyName">Name of the property to validate.</param>
        /// <returns></returns>
        public Task ValidateProperty(object instance, string propertyName)
        {
            return Task.Factory.StartNew(() =>
            {
                //Get the Validation Rule for the property
                var rule = RulesCollection.Where(v => v.PropertyName == propertyName)
                                          .FirstOrDefault();

                if (RulesCollection.Where(v => v.PropertyName == propertyName).Count() == 0 
                    || !Type.Equals(typeof(T), instance.GetType())
                    || instance == null
                    || string.IsNullOrEmpty(propertyName)
                    || rule==null)
                    return;

                //Check all the actions that return errors (result false) and if they aren't in the errors collection add there and raise ErrorChanged
                foreach (KeyValuePair<Func<T,bool>, string> error in rule.RuleActions)
                {
                    lock (locker)
                    {
                        if ((bool)error.Key.DynamicInvoke(instance) == false)
                        {
                            if (!errors.ContainsKey(propertyName))
                                errors.Add(propertyName, new List<string> { error.Value });
                            else if (errors.Where(e => e.Key == propertyName && !e.Value.Contains(error.Value)).Count() == 0)
                            {
                                errors.Where(e => e.Key == propertyName).Select(e => e.Value).FirstOrDefault().ToList().Add(error.Value);
                                Action<string> action = new Action<string>(((T)instance).OnErrorsChanged);
                                action(propertyName);
                            }
                        }
                        else
                        {
                            var deletedError = errors.Where(e => e.Key == propertyName).Select(e=> e.Value).FirstOrDefault();
                            if (deletedError != null)
                            {
                                deletedError.Remove(error.Value);
                                Action<string> action = new Action<string>(((T)instance).OnErrorsChanged);
                                action(propertyName);
                            }
                        }
                    }                    
                }
            });
        }
        
        /// <summary>
        /// Return a validation rule for the property passed as parameter, in case there are no rules for the selected property will be add a new one.
        /// </summary>
        /// <param name="property">Property to look for.</param>
        /// <returns></returns>
        public ValidationRule<T> Property(Expression<Func<T, object>> property)
        {
            var name = property.GetFullMemberName();
            if (RulesCollection.Where(v => v.PropertyName == name && Type.Equals(v.MemberType, typeof(T))).Count() == 0)
                RulesCollection.Add(new ValidationRule<T>(property));
            return RulesCollection.Where(v => v.PropertyName == name && Type.Equals(v.MemberType, typeof(T)))
                           .FirstOrDefault();
        }

        /// <summary>
        /// Merge the rules on the list passed as parameter with the current rules.
        /// </summary>
        /// <typeparam name="T">Type of the validable object.</typeparam>
        /// <param name="instance">Validable Object.</param>
        /// <param name="newRules">Rules to merge.</param>
        /// <param name="mergingAction">How to merge the collections.</param>
        /// <returns></returns>
        public ConcurrentBag<ValidationRule<T>> MergeRules(ConcurrentBag<ValidationRule<T>> newRules, MergingActionEnum mergingAction = MergingActionEnum.ReplaceRules)
        {
            if (RulesCollection == null || newRules == null)
                throw new ArgumentNullException(string.Format("Impossible to merge the rules, instance.IsNull== {0} newRules.IsNull== {1}.", RulesCollection == null, newRules == null));
            switch (mergingAction)
            {
                case MergingActionEnum.MergeAll:
                    var rulesToAdd = newRules.Where(r => !RulesCollection.Contains(r, new ValidationRuleEqualityComparer<T>()));
                    RulesCollection = RulesCollection.Concat(rulesToAdd).ToConcurrentBag();
                    foreach (var rule in RulesCollection.AsParallel())
                    {
                        var newRule = newRules.Where(r => r.PropertyName == rule.PropertyName).DefaultIfEmpty(null);
                        if (newRule != null)
                            rule.MergeRuleActions(newRule.First().RuleActions);
                    }
                    break;
                case MergingActionEnum.ReplaceRules:
                    RulesCollection = newRules;
                    break;
                case MergingActionEnum.ReplacePropertyRules:
                    RulesCollection = RulesCollection.Where(r => !newRules.Contains(r, new ValidationRuleEqualityComparer<T>()))
                                                     .Concat(newRules)
                                                     .ToConcurrentBag();
                    break;
                case MergingActionEnum.OverridePropertyRules:
                    RulesCollection = newRules.Where(r => !RulesCollection.Contains(r, new ValidationRuleEqualityComparer<T>()))
                                              .Concat(RulesCollection)
                                              .ToConcurrentBag();
                    break;
                case MergingActionEnum.MergeExistingRuleActions:
                    var existingRules = RulesCollection.Where(r => newRules.Contains(r, new ValidationRuleEqualityComparer<T>()));
                    foreach (var rule in existingRules.AsParallel())
                        rule.MergeRuleActions(newRules.Where(r => r.PropertyName == rule.PropertyName).FirstOrDefault().RuleActions);
                    break;
            }
            return RulesCollection;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of AsyncValidator.
        /// </summary>
        public AsyncValidator()
        {
            RulesCollection = new ConcurrentBag<ValidationRule<T>>();
            errors = new Dictionary<string, List<string>>();
        }
        #endregion
    }
}
