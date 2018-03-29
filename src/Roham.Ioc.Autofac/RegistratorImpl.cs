using System;
using Autofac;
using IOC = Roham.Lib.Ioc;
using AF = Autofac;

namespace Roham.Ioc.Autofac
{
    internal class RegistratorImpl : IOC.IRegistrator
    {
        private readonly AF.ContainerBuilder _builder;

        public RegistratorImpl(AF.ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            _builder
                .RegisterInstance(instance)
                .As<T>();
        }

        public void RegisterAsSingleInstance<TService, TImplementer>()
        {
            RegisterInternal(typeof(TService), typeof(TImplementer), IOC.LifetimeScopeType.SingleInstance);
        }

        public void RegisterAsSingleInstance(Type serviceType, Type implementerType, string name = null)
        {
            RegisterInternal(serviceType, implementerType, IOC.LifetimeScopeType.SingleInstance, name);
        }

        public void RegisterAsPerRequest<TService, TImplementer>()
        {
            RegisterInternal(typeof(TService), typeof(TImplementer), IOC.LifetimeScopeType.InstancePerRequest);
        }

        public void RegisterAsPerRequest(Type serviceType, Type implementerType, string name = null)
        {
            RegisterInternal(serviceType, implementerType, IOC.LifetimeScopeType.InstancePerRequest, name);
        }

        public void RegisterAsPerLifetimeScope<TService, TImplementer>()
        {
            RegisterInternal(typeof(TService), typeof(TImplementer), IOC.LifetimeScopeType.InstancePerLifetimeScope);
        }

        public void RegisterAsPerLifetimeScope(Type serviceType, Type implementerType, string name = null)
        {
            RegisterInternal(serviceType, implementerType, IOC.LifetimeScopeType.InstancePerLifetimeScope, name);
        }

        public void Register<TService>(Type implementerType)
        {
            RegisterInternal(typeof(TService), implementerType, IOC.LifetimeScopeType.InstancePerDependency);
        }

        public void Register<TService, TImplementer>()
        {
            RegisterInternal(typeof(TService), typeof(TImplementer), IOC.LifetimeScopeType.InstancePerDependency);
        }

        public void Register(Type serviceType, Type implementerType, string name = null)
        {
            RegisterInternal(serviceType, implementerType, IOC.LifetimeScopeType.InstancePerDependency, name);
        }

        private void RegisterInternal(Type serviceType, Type implementerType, IOC.LifetimeScopeType lifetimeScope, string name = null)
        {
            if (implementerType.IsOpenGeneric())
            {
                var registration = _builder
                    .RegisterGeneric(implementerType);
                if (string.IsNullOrEmpty(name))
                    registration
                        .As(serviceType);
                else
                    registration
                        .Named(name, serviceType);

                switch (lifetimeScope)
                {
                    case IOC.LifetimeScopeType.SingleInstance:
                        registration.SingleInstance();
                        break;
                    case IOC.LifetimeScopeType.InstancePerLifetimeScope:
                        registration.InstancePerLifetimeScope();
                        break;
                    case IOC.LifetimeScopeType.InstancePerRequest:
                        registration.InstancePerRequest();
                        break;
                    case IOC.LifetimeScopeType.InstancePerDependency:
                    default:
                        registration.InstancePerDependency();
                        break;
                }
            }
            else
            {
                var registration = _builder
                    .RegisterType(implementerType);
                if (string.IsNullOrEmpty(name))
                    registration
                        .As(serviceType);
                else
                    registration
                        .Named(name, serviceType);

                switch (lifetimeScope)
                {
                    case IOC.LifetimeScopeType.SingleInstance:
                        registration.SingleInstance();
                        break;
                    case IOC.LifetimeScopeType.InstancePerLifetimeScope:
                        registration.InstancePerLifetimeScope();
                        break;
                    case IOC.LifetimeScopeType.InstancePerRequest:
                        registration.InstancePerRequest();
                        break;
                    case IOC.LifetimeScopeType.InstancePerDependency:
                    default:
                        registration.InstancePerDependency();
                        break;
                }
            }
        }
    }
}
