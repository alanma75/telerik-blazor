using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelerikBlazorApp1.Data
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// This is function from Telerik library
        /// It has 1 bug and 1 limitation
        /// 
        /// Bug:
        /// Expression.Property(expression, propertyName) - can raise Exception due to well-known limitation of .Net
        /// grand parent interface property is unseen

        /// Limitation:
        /// This function cannot handle indexed property e.g. first.Seconds[2].Name2
        /// </summary>
        public static Expression<Func<TItem>> GetNestedExpression<TItem>(this object item, string field)
        {
            Expression expression = Expression.Constant(item);
            foreach (string propertyName in field.Split('.'))
                expression = Expression.Property(expression, propertyName);
            Expression convertedExpression = Expression.Convert(expression, typeof(TItem));
            return Expression.Lambda<Func<TItem>>(convertedExpression);
        }

        // this function matches field accessor path using regex(or manual parsing), then either get Property directly 
        // or go deaper to inheritance to find the matching property name. 
        public static Expression<Func<TItem>> GetNestedExpressionEnhanced<TItem>(this object item, string field)
        {
            Expression expression = Expression.Constant(item);
            foreach (string propertyName in field.Split('.'))
            {
                expression = GetComplexExpressionUsingRegex(expression, propertyName);
                //expression = GetComplexExpressionManuallyParsing(expression, propertyName);

            }
            Expression convertedExpression = Expression.Convert(expression, typeof(TItem));
            return Expression.Lambda<Func<TItem>>(convertedExpression);
        }


        /// <summary>
        /// Without Regex
        /// </summary>
        private static Expression GetComplexExpressionManuallyParsing(Expression expression, string propertyName)
        {
            int indexOfBrackets = propertyName.IndexOf('[');

            if (indexOfBrackets < 0)
            {
                expression = GetProperty(expression, propertyName);
            }
            else
            {
                string innerPropertyName = propertyName.Substring(0, indexOfBrackets);
                string propIndex = propertyName.Substring(indexOfBrackets + 1, propertyName.Length - indexOfBrackets - 2);
                int index = int.Parse(propIndex);

                expression = GetProperty(expression, innerPropertyName);

                var constant = Expression.Constant(index);
                expression = GetProperty(expression, "Item", constant);
            }

            return expression;
        }

        private static Regex regex = new Regex(@"(\S+)\[(\d+)\]");
        /// <summary>
        /// With Regex
        /// </summary>
        private static Expression GetComplexExpressionUsingRegex(Expression expression, string propertyName)
        {
            MatchCollection matches = regex.Matches(propertyName);
            if (matches.Count == 0)
            {
                expression = GetProperty(expression, propertyName);
            }
            else
            {
                string innerPropertyName = matches[0].Groups[1].Value;

                string propIndex = matches[0].Groups[2].Value;
                int index = int.Parse(propIndex);

                expression = GetProperty(expression, innerPropertyName);

                ConstantExpression constant = Expression.Constant(index);
                expression = GetProperty(expression, "Item", constant);
            }

            return expression;
        }

        private static Expression GetProperty(Expression expression, string propertyName)
        {
            PropertyInfo propertyInfo = expression.Type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                propertyInfo = GetPropertyFromParents(propertyName, expression.Type);
            }

            return Expression.Property(expression, propertyInfo);
        }

        private static Expression GetProperty(Expression expression, string propertyName, Expression constantExpression)
        {
            PropertyInfo propertyInfo = expression.Type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                propertyInfo = GetPropertyFromParents(propertyName, expression.Type);
            }

            return Expression.Property(expression, propertyInfo, constantExpression);
        }

        private static PropertyInfo GetPropertyFromParents(string propertyName, Type type)
        {
            //TODO: We should start finding in the Generic interfaces and after that in the Non-Generic
            List<Type> list = type.GetInterfaces().OrderByDescending(i => i.IsGenericType).ToList();
            foreach (Type parentType in list)
            {
                PropertyInfo propertyInfo = parentType.GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    return propertyInfo;
                }

                propertyInfo = GetPropertyFromParents(propertyName, parentType);
                if (propertyInfo != null)
                {
                    return propertyInfo;
                }
            }

            return null;
        }
    }

}
