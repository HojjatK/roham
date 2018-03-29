using System;
using System.Collections.Generic;

namespace Roham.Lib.Ioc
{
    public interface IResolver
    {
        T Resolve<T>();
        T Resolve<T>(params DependencyInstance[] dependencies);

        IEnumerable<T> ResolveAll<T>();

        object Resolve(Type type);
        object Resolve(Type type, params DependencyInstance[] dependencies);
    }
}
