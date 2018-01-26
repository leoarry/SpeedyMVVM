using SpeedyMVVM.Expressions;
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
    public static class RuleActionsExtentionMethods
    {
        #region Validation Extension Methods
        /// <summary>
        /// Mark the property as required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="message">Error message (default=Property {PropertyName} is required!)</param>
        /// <returns></returns>
        public static ValidationRule<T> IsRequired<T>(this ValidationRule<T> instance, string message = "") where T:ValidableObject
        {
            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));
            //Create the comparison.
            var ex = member.IsNotNull().AndAlso(Expression.Not(member.IsNullOrEmptyString()));
            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T, bool>>(ex, item).Compile();
            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} is required!", instance.PropertyName);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property has max length than the value passed as parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="MaxLenght">Max length of the string</param>
        /// <param name="message">Error message (default=Property {PropertyName} must be maximum {MaxLenght}!)</param>
        /// <returns></returns>
        public static ValidationRule<T> HasMaxLength<T>(this ValidationRule<T> instance, int MaxLenght, string message = "") where T : ValidableObject
        {
            if (!MaxLenght.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");

            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a empty string in case the value is null
            member = Expression.Coalesce(member, Expression.Constant("", typeof(string)));

            var stringLength = Expression.Property(member, "Length");
            //Create the comparison.
            var ex = stringLength.IsLessThanOrEqual(Expression.Constant(MaxLenght));
            
            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} must be maximum {1}!", instance.PropertyName, MaxLenght);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is greater than the value passed as parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="value">Value to compare.</param>
        /// <param name="message">Error message (default='Property {PropertyName} must be greater than {value}!')</param>
        /// <returns></returns>
        public static ValidationRule<T> IsGreaterThan<T>(this ValidationRule<T> instance, object value, string message = "") where T : ValidableObject
        {
            if (!value.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");
            
            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property          
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item,(c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable 
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsGreaterThan(Expression.Constant(value));

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} must be greater than {1}!", instance.PropertyName, value);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is greater than the value passed as parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="value">Value to compare.</param>
        /// <param name="message">Error message (default='Property {PropertyName} must be greater than {value}!')</param>
        /// <returns></returns>
        public static ValidationRule<T> IsGreaterThanOrEqual<T>(this ValidationRule<T> instance, object value, string message = "") where T : ValidableObject
        {
            if (!value.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");

            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");
            
            //Get the member in case of nested property                 
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable      
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsGreaterThanOrEqual(Expression.Constant(value));

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} must be greater than {1}!", instance.PropertyName, value);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is less than the value passed as parameter (Value MUST be a number).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="value">Value to compare.</param>
        /// <param name="message">Error message (default='Property {PropertyName} must be less than {value}!')</param>
        /// <returns></returns>
        public static ValidationRule<T> IsLessThan<T>(this ValidationRule<T> instance, object value, string message = "") where T : ValidableObject
        {
            if (!value.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");
            
            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable 
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsLessThan(Expression.Constant(value));

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} must be less than {1}!", instance.PropertyName, value);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is less than the value passed as parameter (Value MUST be a number).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="value">Value to compare.</param>
        /// <param name="message">Error message (default='Property {PropertyName} must be less than {value}!')</param>
        /// <returns></returns>
        public static ValidationRule<T> IsLessThanOrEqual<T>(this ValidationRule<T> instance, object value, string message = "") where T : ValidableObject
        {
            if (!value.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");

            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable 
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsLessThanOrEqual(Expression.Constant(value));

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} must be less than {1}!", instance.PropertyName, value);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is within a range MIN-MAX.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="LowLimit">Minimum limit.</param>
        /// <param name="HighLimit">Maximum limit.</param>
        /// <param name="message">Error message (default=Property {PropertyName} over limits! LowLimit: {LowLimit}, UpLimit:{UpLimit})</param>
        /// <returns></returns>
        public static ValidationRule<T> IsValueInRange<T>(this ValidationRule<T> instance, object LowLimit, object HighLimit, string message = "") where T : ValidableObject
        {
            if (!LowLimit.ToString().IsNumber() || !HighLimit.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");

            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsLessThanOrEqual(Expression.Constant(HighLimit));
            ex = Expression.AndAlso(ex, member.IsGreaterThanOrEqual(Expression.Constant(LowLimit)));

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} over limits! LowLimit: {1}, HighLimit:{2}", instance.PropertyName, LowLimit, HighLimit);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }

        /// <summary>
        /// Check if the property value is within a range MIN-MAX.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="LowLimit">Minimum limit.</param>
        /// <param name="HighLimit">Maximum limit.</param>
        /// <param name="message">Error message (default=Property {PropertyName} over limits! LowLimit: {LowLimit}, UpLimit:{UpLimit})</param>
        /// <returns></returns>
        public static ValidationRule<T> IsValueOutRange<T>(this ValidationRule<T> instance, object LowLimit, object HighLimit, string message = "") where T : ValidableObject
        {
            if (!LowLimit.ToString().IsNumber() || !HighLimit.ToString().IsNumber())
                throw new InvalidCastException("Impossible convert the value in a number.");

            //Setup the 'left' expression parameter.
            var item = Expression.Parameter(instance.MemberType, "item");

            //Get the member in case of nested property         
            Expression member = instance.PropertyName.Split('.')
                                                         .Aggregate<string, Expression>
                                                         (item, (c, m) => Expression.Property(c, m));

            //Return a default value in case the value is null and nullable
            if (member.Type.IsNullableType())
                member = Expression.Coalesce(member, Expression.Constant(default(int), typeof(int)));

            //Create the comparison.
            var ex = member.IsLessThanOrEqual(Expression.Constant(HighLimit));
            ex = Expression.AndAlso(ex, member.IsGreaterThanOrEqual(Expression.Constant(LowLimit)));
            ex = Expression.Not(ex);

            //Create the lambda expression.
            var lambdaExp = Expression.Lambda<Func<T,bool>>(ex, item).Compile();

            if (!instance.RuleActions.ContainsKey(lambdaExp))
            {
                if (string.IsNullOrEmpty(message))
                    message = string.Format("Property {0} in limits! LowLimit: {1}, HighLimit:{2}", instance.PropertyName, LowLimit, HighLimit);
                instance.RuleActions.TryAdd(lambdaExp, message);
            }
            return instance;
        }
        #endregion
        
    }
}
