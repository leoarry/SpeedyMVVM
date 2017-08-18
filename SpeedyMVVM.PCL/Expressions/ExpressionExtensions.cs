using SpeedyMVVM.Expressions.Visitors;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SpeedyMVVM.Expressions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combine two expression using the logic described into the parameter 'ExpressionConcatEnum'
        /// </summary>
        /// <typeparam name="T">Type for the expression.</typeparam>
        /// <param name="exp">First expression.</param>
        /// <param name="newExp">Expression to combine with.</param>
        /// <returns>Return a new expression combined in AND.</returns>
        public static Expression<Func<T, bool>> Concat<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp, ExpressionConcatEnum en)
        {
            // get the visitor
            var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
            // replace the parameter in the expression just created
            newExp = visitor.Visit(newExp) as Expression<Func<T, bool>>;
            BinaryExpression binExp = null;
            switch (en)
            {
                case ExpressionConcatEnum.And:
                    binExp = Expression.And(exp.Body, newExp.Body);
                    break;
                case ExpressionConcatEnum.AndNot:
                    binExp = Expression.And(exp.Body, Expression.Not(newExp.Body));
                    break;
                case ExpressionConcatEnum.Or:
                    binExp = Expression.OrElse(exp.Body, newExp.Body);
                    break;
                case ExpressionConcatEnum.OrNot:
                    binExp = Expression.OrElse(exp.Body, Expression.Not(newExp.Body));
                    break;
            }
            return Expression.Lambda<Func<T, bool>>(binExp, newExp.Parameters);
        }

        /// <summary>
        /// Combine two Expression in an unique Expression Tree.
        /// </summary>
        /// <typeparam name="T">Type of the Expression Tree.</typeparam>
        /// <typeparam name="TIntermediate1">Type of the first Intermediate</typeparam>
        /// <typeparam name="TIntermediate2">Type of the second Intermediate</typeparam>
        /// <typeparam name="TResult">Type of the result selector</typeparam>
        /// <param name="first">First expression to combine</param>
        /// <param name="second">Second expression to combine</param>
        /// <param name="resultSelector">Result selector for the combination method. Example: '(a, b) => a + "," + b)' </param>
        /// <returns>Return an Expression which is the result of the combined expressions.</returns>
        public static Expression<Func<T, TResult>> Combine<T, TIntermediate1, TIntermediate2, TResult>
                (this Expression<Func<T, TIntermediate1>> first,
                Expression<Func<T, TIntermediate2>> second,
                Expression<Func<TIntermediate1, TIntermediate2, TResult>> resultSelector)
        {
            var param = Expression.Parameter(typeof(T));
            var body = resultSelector.Body.Replace(
                    resultSelector.Parameters[0],
                    first.Body.Replace(first.Parameters[0], param))
                .Replace(
                    resultSelector.Parameters[1],
                    second.Body.Replace(second.Parameters[0], param));
            return Expression.Lambda<Func<T, TResult>>(body, param);
        }

        /// <summary>
        /// Replace the visitor in the Expression.
        /// </summary>
        /// <param name="expression">Expression to elaborate.</param>
        /// <param name="searchEx">Param to be replaced.</param>
        /// <param name="replaceEx">New value for the Param to be replaced.</param>
        /// <returns>Return an Expression with the value replaced.</returns>
        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }
    }
}
