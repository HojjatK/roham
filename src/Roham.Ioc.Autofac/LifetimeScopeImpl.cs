using System;
using IOC = Roham.Lib.Ioc;
using AF = Autofac;

namespace Roham.Ioc.Autofac
{
    internal class LifetimeScopeImpl : ResolverImpl, IOC.ILifetimeScope
    {
        private readonly AF.ILifetimeScope _lifetimeScope;

        public LifetimeScopeImpl(AF.ILifetimeScope lifetimeScope)
            : base(lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IOC.ILifetimeScope BeginLifetimeScope()
        {
            var result = new LifetimeScopeImpl(_lifetimeScope.BeginLifetimeScope());
            return result;
        }

        public IOC.ILifetimeScope BeginLifetimeScope(object tag)
        {
            var result = new LifetimeScopeImpl(_lifetimeScope.BeginLifetimeScope(tag));
            return result;
        }

        public IOC.ILifetimeScope BeginLifetimeScope(Action<IOC.IRegistrator> registations)
        {
            var childScope = _lifetimeScope.BeginLifetimeScope(
                builder =>
                {
                    registations(new RegistratorImpl(builder));
                });
            var result = new LifetimeScopeImpl(childScope);
            return result;
        }

        public IOC.ILifetimeScope BeginLifetimeScope(object tag, Action<IOC.IRegistrator> registations)
        {
            var childScope = _lifetimeScope.BeginLifetimeScope(
                tag,
                builder =>
                {
                    registations(new RegistratorImpl(builder));
                });
            var result = new LifetimeScopeImpl(childScope);
            return result;
        }

        private readonly object _disposeLock = new object();
        private volatile bool _isDisposed = false;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                lock (_disposeLock)
                {
                    if (!_isDisposed)
                    {
                        _lifetimeScope.Dispose();
                        _isDisposed = true;
                    }
                }
            }
        }
    }
}
