using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    public class ValidationRule<T>
    {
        #region Private Fileds
        private Expression<Func<T, object>> _Property;
        private Type _MemberType;
        private string _PropertyName;
        #endregion

        #region Properties
        /// <summary>
        /// Expression representing the property.
        /// </summary>
        public Expression<Func<T,object>> Property { get { return _Property; } }

        /// <summary>
        /// Get the type of the member for the property expression.
        /// </summary>
        public Type MemberType { get { return _MemberType; } }
        
        /// <summary>
        /// Get the full name of the property to be validated in case the property is nested.
        /// </summary>
        public string PropertyName { get { return _PropertyName; } }

        /// <summary>
        /// Collection of actions and error messages for the property
        /// </summary>
        public ConcurrentDictionary<Func<T,bool>, string> RuleActions { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Will clear the collection of actions for this rule.
        /// </summary>
        public void ClearRuleActions()
        {
            RuleActions.Clear();
        }

        /// <summary>
        /// Add a new action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Action to add.</param>
        /// <param name="message">Error message for teh action.</param>
        public void AddRuleAction(Func<T,bool> action, string message)
        {
            RuleActions.GetOrAdd(action, message);
        }

        /// <summary>
        /// Remove the action passed as parameter and the related error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Action to remove.</param>
        public void RemoveRuleAction(Func<T,bool> action)
        {
            string val="";
            RuleActions.TryRemove(action, out val);
        }

        /// <summary>
        /// Merge the actions passed as parameter with the validation rule actions, adding only the missing rules.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newActions">Collection of actions to merge.</param>
        public void MergeRuleActions(Dictionary<Func<T, bool>, string> newActions)
        {
            if (!Type.Equals(typeof(T), MemberType))
                throw new MemberAccessException("Missmatch with stored type and expression type, impossible to merge the actions!");
            if (newActions == null)
                return;
            var rulesToAdd = newActions.Where(r => !RuleActions.ContainsKey(r.Key))
                                       .Cast<KeyValuePair<Func<T,bool>, string>>();
            RuleActions = RuleActions.Concat(rulesToAdd.ToConcurrentDictionary())
                                     .ToConcurrentDictionary();
        }

        /// <summary>
        /// Merge the actions passed as parameter with the validation rule actions, adding only the missing rules.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newActions">Collection of actions to merge.</param>
        public void MergeRuleActions(ConcurrentDictionary<Func<T, bool>, string> newActions)
        {
            if (!Type.Equals(typeof(T), MemberType))
                throw new MemberAccessException("Missmatch with stored type and expression type, impossible to merge the actions!");
            if (newActions == null)
                return;
            var rulesToAdd = newActions.Where(r => !RuleActions.ContainsKey(r.Key))
                                       .Cast<KeyValuePair<Func<T,bool>, string>>();
            RuleActions = RuleActions.Concat(rulesToAdd.ToConcurrentDictionary())
                                     .ToConcurrentDictionary();
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of ValidationRule for the property passed as parameter.
        /// </summary>
        /// <param name="property">Lambda expression representing the property.</param>
        public ValidationRule(Expression<Func<T,object>> property)
        {
            _Property = property;
            _MemberType = property.GetMemberType();
            _PropertyName = property.GetFullMemberName();
            RuleActions = new ConcurrentDictionary<Func<T,bool>, string>();
        }
        #endregion
    }
}
