using System;
using System.Collections.Generic;

namespace Roham.Lib.Ioc
{
    public abstract class AbstractDependencyResolver : ILifetimeScope
    {
        protected AbstractDependencyResolver(ILifetimeScope rootScope)
        {
            RootScope = rootScope;
        }

        protected ILifetimeScope RootScope { get; }

        public ILifetimeScope BeginLifetimeScope()
        {
            return RootScope.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return RootScope.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<IRegistrator> registations)
        {
            return RootScope.BeginLifetimeScope(registations);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<IRegistrator> registations)
        {
            return RootScope.BeginLifetimeScope(tag, registations);
        }

        public T Resolve<T>()
        {
            return RootScope.Resolve<T>();
        }

        public T Resolve<T>(params DependencyInstance[] dependencies)
        {
            return RootScope.Resolve<T>(dependencies);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return RootScope.ResolveAll<T>();
        }

        public object Resolve(Type type)
        {
            return RootScope.Resolve(type);
        }

        public object Resolve(Type type, params DependencyInstance[] dependencies)
        {
            return RootScope.Resolve(type, dependencies);
        }

        private volatile bool _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (RootScope != null)
                {
                    RootScope.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}