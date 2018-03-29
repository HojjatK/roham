using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        public static bool IsOpenGeneric(this Type type)
        {
            // Open generic types are always generic and 
            // always have a type equal the results of GetGenericTypeDefinition()
            return (type.IsGenericType && type.GetGenericTypeDefinition().Equals(type));
        }

        public static Type CreateGenericType(this Type genericDefType, params Type[] typeArgs)
        {
            return genericDefType.MakeGenericType(typeArgs);
        }

        public static T CastTo<T>(this object instance)
        {
            return (T)instance;
        }

        public static IEnumerable<Type> GetTypesUpInheritanceHierarchy(this Type type)
        {
            if (type == typeof(object) || type == null)
            {
                yield break;
            }

            yield return type;

            foreach (var baseType in type.BaseType.GetTypesUpInheritanceHierarchy())
            {
                yield return baseType;
            }
        }

        public static IEnumerable<FieldInfo> GetFieldsUpInheritanceHierarchy(this Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.IsPrivate || field.DeclaringType == type)
                {
                    yield return field;
                }
            }
            foreach (var t in type.BaseType.GetTypesUpInheritanceHierarchy())
            {
                foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (!field.IsFamily)
                    {
                        yield return field;
                    }
                }
            }
        }

        public static object CreateInstance(this Type type)
        {
            var ctor = type.GetConstructorFunc();
            return ctor();
        }

        public static object CreateInstance<P1>(this Type type, P1 p1)
        {
            var ctor = type.GetConstructorFunc<P1>();
            return ctor(p1);
        }

        public static object CreateInstance<P1, P2>(this Type type, P1 p1, P2 p2)
        {
            var ctor = type.GetConstructorFunc<P1, P2>();
            return ctor(p1, p2);
        }

        public static object CreateInstance<P1, P2, P3>(this Type type, P1 p1, P2 p2, P3 p3)
        {
            var ctor = type.GetConstructorFunc<P1, P2, P3>();
            return ctor(p1, p2, p3);
        }

        private static Func<object> GetConstructorFunc(this Type type)
        {
            var constructorInfo = type.GetConstructor(Type.EmptyTypes);
            var ctor = Expression.Lambda<Func<object>>(Expression.New(constructorInfo)).Compile();
            return ctor;
        }

        private static Func<P1, object> GetConstructorFunc<P1>(this Type type)
        {
            var p1 = Expression.Parameter(typeof(P1), "p1");
            var constructorInfo = type.GetConstructor(new[] { typeof(P1) });
            var argsExp = new[] { p1 };
            var ctor =
                Expression.Lambda<Func<P1, object>>(Expression.New(constructorInfo, argsExp), new[] { p1 }).Compile();
            return ctor;
        }

        private static Func<P1, P2, object> GetConstructorFunc<P1, P2>(this Type type)
        {
            var p1 = Expression.Parameter(typeof(P1), "p1");
            var p2 = Expression.Parameter(typeof(P2), "p2");
            var constructorInfo = type.GetConstructor(new[] { typeof(P1), typeof(P2) });
            var argsExp = new[] { p1, p2 };
            var ctor =
                Expression.Lambda<Func<P1, P2, object>>(Expression.New(constructorInfo, argsExp), new[] { p1, p2 })
                    .Compile();
            return ctor;
        }

        private static Func<P1, P2, P3, object> GetConstructorFunc<P1, P2, P3>(this Type type)
        {
            var p1 = Expression.Parameter(typeof(P1), "p1");
            var p2 = Expression.Parameter(typeof(P2), "p2");
            var p3 = Expression.Parameter(typeof(P3), "p3");
            var constructorInfo = type.GetConstructor(new[] { typeof(P1), typeof(P2), typeof(P3) });
            var argsExp = new[] { p1, p2, p3 };
            var ctor =
                Expression.Lambda<Func<P1, P2, P3, object>>(Expression.New(constructorInfo, argsExp),
                                                            new[] { p1, p2, p3 }).Compile();
            return ctor;
        }
    }
}
