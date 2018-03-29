using System;

namespace Roham.Lib.Ioc
{
    public class DependencyInstance
    {
        public DependencyInstance(Type type, object instance)
        {
            Type = type;
            Instance = instance;
        }

        public Type Type { get; }

        public object Instance { get; }
    }
}
