using System.Reflection;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T>> propertyGetExpression)
        {
            MemberExpression body = propertyGetExpression.Body as MemberExpression;
            if (body == null)
            {
                body = ((UnaryExpression)propertyGetExpression.Body).Operand as MemberExpression;
            }

            if (body == null)
                throw new ArgumentException("Invalid property expression", "propertyGetExpression");

            return body;
        }

        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> propertyGetExpression)
        {
            return (PropertyInfo)propertyGetExpression.GetMemberExpression().Member;
        }

        public static PropertyInfo GetPropertyInfo<TInstance, T>(this Expression<Func<TInstance, T>> propertyGetExpression)
        {
            return (PropertyInfo)((MemberExpression)propertyGetExpression.Body).Member;
        }

        public static string GetName<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new NullReferenceException("Expression is null");

            MemberExpression body = expression.Body as MemberExpression;
            if (body == null)
            {
                var lambda = expression as LambdaExpression;
                if (lambda.Body.NodeType == ExpressionType.Constant)
                {
                    return "";
                }

                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = ubody.Operand as MemberExpression;

                if (body == null)
                    throw new ArgumentException("Expression is invalid");
            }
            return body.Member.Name;
        }
    }
}
