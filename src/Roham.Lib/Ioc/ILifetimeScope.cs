using System;

namespace Roham.Lib.Ioc
{
    public enum LifetimeScopeType
    {
        SingleInstance,
        InstancePerRequest,
        InstancePerLifetimeScope,
        InstancePerDependency,
    }

    public interface ILifetimeScope : IResolver, IDisposable
    {
        ILifetimeScope BeginLifetimeScope();
        ILifetimeScope BeginLifetimeScope(object tag);
        ILifetimeScope BeginLifetimeScope(Action<IRegistrator> registations);
        ILifetimeScope BeginLifetimeScope(object tag, Action<IRegistrator> registations);
    }
}