using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SpeedyMVVM.Expressions
{
    public static class ComparisonExpressionExtensions
    {
        /// <summary>
        /// Create an expression that represent a 'great than' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsGreaterThan(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.GreaterThan(e1, e2);
        }

        /// <summary>
        /// Create an expression that represent a 'less than' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsLessThan(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.LessThan(e1, e2);
        }

        /// <summary>
        /// Create an expression that represent a 'great than or equal' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsGreaterThanOrEqual(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.GreaterThanOrEqual(e1, e2);
        }

        /// <summary>
        /// Create an expression that represent a 'less than or equal' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsLessThanOrEqual(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.LessThanOrEqual(e1, e2);
        }

        /// <summary>
        /// Create an expression that represent a 'equal' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsEqual(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.Equal(e1, e2);
        }

        /// <summary>
        /// Create an expression that represent a 'not equal' comparison.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <param name="e2">second expression</param>
        /// <returns></returns>
        public static Expression IsDifferent(this Expression e1, Expression e2)
        {
            if (e1.Type.IsNullableType() && !e2.Type.IsNullableType())
                e2 = Expression.Convert(e2, e1.Type);
            else if (!e1.Type.IsNullableType() && e2.Type.IsNullableType())
                e1 = Expression.Convert(e1, e2.Type);
            return Expression.NotEqual(e1, e2);
        }

        /// <summary>
        /// Create an expression which compare using 'equal'comparison the parameter expression with a null constant.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <returns></returns>
        public static Expression IsNull(this Expression e1)
        {
            return e1.IsEqual(Expression.Constant(null, e1.Type));
        }

        /// <summary>
        /// Create an expression which compare using 'not equal'comparison the parameter expression with a null constant.
        /// </summary>
        /// <param name="e1">current expression</param>
        /// <returns></returns>
        public static Expression IsNotNull(this Expression e1)
        {
            return e1.IsDifferent(Expression.Constant(null, e1.Type));
        }

        /// <summary>
        /// Convert to string and compare if null or empty string.
        /// </summary>
        /// <param name="e1"></param>
        /// <returns></returns>
        public static Expression IsNullOrEmptyString(this Expression e1)
        {
            var toString = Expression.Call(e1, ExpressionExtensions.ToStringMethod);
            return Expression.Call(ExpressionExtensions.IsNullOrEmptyMethod, toString);
        }
        
    }
}
