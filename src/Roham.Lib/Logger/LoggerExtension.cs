using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Roham.Lib.Logger
{
    public static class LoggerExtension
    {
        public static void DebugMethodParams(this ILogger logger, params Expression<Func<object>>[] providedParams)
        {
            if (logger.IsDebugEnabled)
            {
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();

                var methodParams = method.GetParameters()
                    .Select(p => new { p.Name, p.ParameterType })
                    .ToDictionary(i => i.Name);

                var providedParametars = new List<Tuple<string, object>>();
                foreach (var aExpression in providedParams)
                {
                    Expression bodyType = aExpression.Body;
                    if (bodyType is MemberExpression)
                    {
                        providedParametars.Add(GetProvidedParamaterDetail((MemberExpression)aExpression.Body));
                    }
                    else if (bodyType is UnaryExpression)
                    {
                        UnaryExpression unaryExpression = (UnaryExpression)aExpression.Body;
                        providedParametars.Add(GetProvidedParamaterDetail((MemberExpression)unaryExpression.Operand));
                    }
                }
                string providedParamsStr = string.Join(",", providedParametars.Select(p => string.Format("", p.Item1, (p.Item2 ?? "null").ToString())));
                logger.Debug(string.Format("{0}::{1}({2}) called", method.DeclaringType.FullName, method.Name, providedParamsStr.Trim()));
            }
        }

        private static Tuple<string, object> GetProvidedParamaterDetail(MemberExpression memberExpression)
        {
            ConstantExpression constantExpression = (ConstantExpression)memberExpression.Expression;
            var name = memberExpression.Member.Name;
            var value = ((FieldInfo)memberExpression.Member).GetValue(constantExpression.Value);
            return new Tuple<string, object>(name, value);
        }
    }
}
