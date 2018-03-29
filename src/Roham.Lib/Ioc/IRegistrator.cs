using System;

namespace Roham.Lib.Ioc
{
    public interface IRegistrator
    {
        void RegisterInstance<T>(T instance) where T : class;
        void RegisterAsSingleInstance<TService, TImplementer>();
        void RegisterAsSingleInstance(Type serviceType, Type implementerType, string name = null);

        void RegisterAsPerRequest<TService, TImplementer>();
        void RegisterAsPerRequest(Type serviceType, Type implementerType, string name = null);

        void RegisterAsPerLifetimeScope<TService, TImplementer>();
        void RegisterAsPerLifetimeScope(Type serviceType, Type implementerType, string name = null);

        void Register<TService>(Type implementerType);
        void Register<TService, TImplementer>();
        void Register(Type serviceType, Type implementerType, string name = null);
    }
}