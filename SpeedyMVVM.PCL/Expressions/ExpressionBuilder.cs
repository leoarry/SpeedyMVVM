using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SpeedyMVVM.Expressions
{
    /// <summary>
    /// Provide static methods to generate expressions.
    /// </summary>
    public static class ExpressionBuilder
    {

        #region Static Fields
        private static MethodInfo containsMethod = typeof(string).GetTypeInfo().GetDeclaredMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetRuntimeMethod("StartsWith", new[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetRuntimeMethod("EndsWith", new[] { typeof(string) });
        private static MethodInfo isNullOrEmptyMethod = typeof(string).GetRuntimeMethod("IsNullOrEmpty", new[] { typeof(string) });
        #endregion

        #region Static Methods
        /// <summary>
        /// Get an Expression based on the parameter 'filters'.
        /// </summary>
        /// <typeparam name="T">Type for the expression.</typeparam>
        /// <param name="filters">List of IExpressionModel filters.</param>
        /// <returns>Return the Expression.</returns>
        public static Expression<Func<T, bool>> GetExpression<T>(IList<ExpressionModel> filters)
        {
            if (filters == null) return null;
            filters = filters.Where(filter => filter != null && !(string.IsNullOrEmpty(filter.PropertyName))).ToList();
            if (!filters.Any()) return null;
            var item = Expression.Parameter(typeof(T), "item");
            Expression<Func<T, bool>> exp = null;
            Expression<Func<T, bool>> newExp = null;
            foreach(var f in filters)
            {
                if (exp == null)
                    exp = Expression.Lambda<Func<T, bool>>(MakePredicate<T>(item, f),item);
                else
                {
                    newExp = Expression.Lambda<Func<T, bool>>(MakePredicate<T>(item, f),item);
                    exp = exp.Concat(newExp, f.ConcatOperator);
                }
            }
            return exp;
        }

        /// <summary>
        /// Private method to get the predicate.
        /// </summary>
        /// <typeparam name="T">Type for the method.</typeparam>
        /// <param name="param">parameter.</param>
        /// <param name="filter">filter.</param>
        /// <returns>Returned Expression.</returns>
        private static Expression MakePredicate<T>(ParameterExpression param, ExpressionModel filter)
        {
            PropertyInfo prop = null;
            Expression member = param;
            Expression constant = null;
            string pName;

            if (filter.PropertyName.Contains("."))
            {
                var fields = filter.PropertyName.Split('.');
                FieldInfo lastField = member.Type.GetRuntimeFields().Where(f => f.FieldType.Name == fields[0]).First(); ;
                for (int i = 0; i < fields.Count(); i++)
                {
                    if (i < fields.Count() - 1)
                        lastField = member.Type.GetRuntimeFields().Where(f => f.FieldType.Name == fields[i]).First();
                    member = Expression.PropertyOrField(member, fields[i]);
                }
                prop = lastField.FieldType.GetRuntimeProperty(fields.Last());
            }
            else
            {
                prop = typeof(T).GetRuntimeProperty(filter.PropertyName);
                member = Expression.Property(param, prop.Name);
            }

            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
            {
                var t = Nullable.GetUnderlyingType(prop.PropertyType);
                if (filter.Value == null)
                    pName = "null";
                else
                    pName = t.Name + "?";
            }
            else if (prop.PropertyType.GetTypeInfo().BaseType == typeof(Enum))
            {
                if (filter.Value == null)
                    pName = "null";
                else
                    pName = "Enum";
            }
            else { pName = prop.PropertyType.Name; }

            switch (pName)
            {
                case "null":
                    constant = Expression.Constant(null, typeof(object));
                    break;
                case "Int32":
                    int val;
                    int.TryParse(filter.Value.ToString(), out val);
                    constant = Expression.Constant(val);
                    break;
                case "Int32?":
                    int v;
                    int.TryParse(filter.Value.ToString(), out v);
                    constant = Expression.Convert(Expression.Constant(v), prop.PropertyType);
                    break;
                case "DateTime":
                    DateTime dt;
                    DateTime.TryParse(filter.Value.ToString(), out dt);
                    constant = Expression.Constant(dt);
                    break;
                case "DateTime?":
                    DateTime dt1;
                    DateTime.TryParse(filter.Value.ToString(), out dt1);
                    constant = Expression.Convert(Expression.Constant(dt1), prop.PropertyType);
                    break;
                case "Single":
                    float sval;
                    float.TryParse(filter.Value.ToString(), out sval);
                    constant = Expression.Constant(sval);
                    break;
                case "Single?":
                    float snval;
                    float.TryParse(filter.Value.ToString(), out snval);
                    constant = Expression.Convert(Expression.Constant(snval), prop.PropertyType);
                    break;
                case "Enum":
                    var enumType = Enum.GetUnderlyingType(prop.PropertyType);
                    member = Expression.Convert(member, enumType);
                    constant = Expression.Convert(Expression.Constant(filter.Value), enumType);
                    break;
                default:
                    constant = Expression.Convert(Expression.Constant(filter.Value), prop.PropertyType);//Expression.Constant(filter.Value);
                    break;
            }
            switch (filter.Operator)
            {
                case ExpressionOperatorEnum.Equals:
                    return Expression.Equal(member, constant);
                case ExpressionOperatorEnum.Different:
                    return Expression.NotEqual(member, constant);
                case ExpressionOperatorEnum.GreaterThan:
                    return Expression.GreaterThan(member, constant);
                case ExpressionOperatorEnum.LessThan:
                    return Expression.LessThan(member, constant);
                case ExpressionOperatorEnum.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);
                case ExpressionOperatorEnum.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);
                case ExpressionOperatorEnum.Contains:
                    return Expression.Call(member, containsMethod, constant);
                case ExpressionOperatorEnum.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);
                case ExpressionOperatorEnum.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);
                case ExpressionOperatorEnum.IsNullOrEmpty:
                    return Expression.Call(member, isNullOrEmptyMethod, constant);
            }

            return null;
        }
        #endregion
    }
}
