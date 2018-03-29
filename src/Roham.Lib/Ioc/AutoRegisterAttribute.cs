using System;

namespace Roham.Lib.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoRegisterAttribute : Attribute
    {
        public AutoRegisterAttribute() : this(null) { }

        public AutoRegisterAttribute(string name)
        {
            Name = name;
            LifetimeScope = LifetimeScopeType.InstancePerDependency;
        }

        public LifetimeScopeType LifetimeScope { get; set; }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoRegistrationExcludeAttribute : Attribute
    {
    }

    public static partial class TypeExtenstions
    {
        public static Tuple<LifetimeScopeType, string> GetRegistrationInfoFromCustomAttribues(this Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(AutoRegisterAttribute), false);
            if (attrs != null)
            {
                foreach (AutoRegisterAttribute attr in attrs)
                {
                    return new Tuple<LifetimeScopeType, string>(attr.LifetimeScope, attr.Name);
                }
            }
            return new Tuple<LifetimeScopeType, string>(LifetimeScopeType.InstancePerDependency, null);
        }
    }
}