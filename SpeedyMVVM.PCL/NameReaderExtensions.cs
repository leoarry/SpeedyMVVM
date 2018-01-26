using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM
{
    public static class NameReaderExtensions
    {
        private static readonly string expressionCannotBeNullMessage = "The expression cannot be null.";
        private static readonly string invalidExpressionMessage = "Invalid expression.";

        #region GetMemberName Methods
        public static MemberExpression GetMemberExpression(this Expression instance)
        {
            MemberExpression expression = null;
            if (instance is UnaryExpression) expression = GetMemberExpression(((UnaryExpression)instance).Operand);
            else if (instance is MemberExpression) expression = (MemberExpression)instance;
            else if (instance is LambdaExpression) expression = GetMemberExpression(((LambdaExpression)instance).Body);
            else throw new InvalidCastException("Expression must be UnaryExpression or MemberExpression or LambdaExpression.");
            return expression; 
        }

        public static string GetFullMemberName(this Expression instance)
        {
            string result = "";
            var expressionsToCheck = new List<MemberExpression>();

            var expression = instance.GetMemberExpression();

            while (expression != null)
            {
                expressionsToCheck.Add(expression);
                expression = expression.Expression as MemberExpression;
            }
            
            for (var i = expressionsToCheck.Count - 1; i >= 0; i--)
            {
                var member = expressionsToCheck[i].Member;
                if (member is PropertyInfo) result = string.Concat(result,(member as PropertyInfo).Name,".");
                else if (member is FieldInfo) result = string.Concat(result, (member as FieldInfo).Name, ".");
                else throw new InvalidCastException("Member must be a property or a field!");
            }
            return result.Remove(result.Length - 1);
        }

        public static string GetMemberName<T, TProp>(this Expression<Func<T, TProp>> instance)
        {
            return GetMemberName(instance.Body);
        }

        public static string GetMemberName<T, TProp>(this T instance, Expression<Func<T, TProp>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }
            return memberNames;
        }

        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMemberName(expression.Body);
        }
        
        public static string GetMemberName(this Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }
            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }
            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }
            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }
            if(expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)expression;
                return GetMemberName(lambdaExpression.Body);
            }
            throw new ArgumentException(invalidExpressionMessage);
        }
        #endregion

        #region GetType Methods

        public static Type GetPropertyType<T, TProp>(this T instance, Expression<Func<T, TProp>> expression)
        {
            return GetMemberType(expression);
        }

        public static Type GetMemberType(this Expression expression)
        {
            MemberExpression member = null;

            if (expression == null)
                throw new ArgumentException(expressionCannotBeNullMessage);

            if (expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)expression;
                return lambdaExpression.Parameters[0].Type;                
            }
            else
                member = expression as MemberExpression;

            if (member == null)
                throw new ArgumentException("'expression' should be a member expression or lambda expression.");

            var propertyInfo = (PropertyInfo)member.Member;

            return propertyInfo.PropertyType;
        }
        #endregion
        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}
