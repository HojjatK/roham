using System;
using System.Collections.Generic;
using Autofac;
using IOC = Roham.Lib.Ioc;
using AF = Autofac;

namespace Roham.Ioc.Autofac
{
    internal abstract class ResolverImpl : IOC.IResolver
    {
        protected readonly AF.IComponentContext _iocContext;

        public ResolverImpl(AF.IComponentContext iocContext)
        {
            if (iocContext == null)
            {
                throw new NotImplementedException("IOC Component Context is null");
            }
            _iocContext = iocContext;
        }

        public T Resolve<T>()
        {
            return _iocContext.Resolve<T>();
        }

        public T Resolve<T>(params IOC.DependencyInstance[] dependencies)
        {
            var parameters = new List<AF.TypedParameter>();
            foreach (var dep in dependencies)
            {
                parameters.Add(new AF.TypedParameter(dep.Type, dep.Instance));
            }
            return _iocContext.Resolve<T>(parameters);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _iocContext.Resolve<IEnumerable<T>>();
        }

        public object Resolve(Type type)
        {
            return _iocContext.Resolve(type);
        }

        public object Resolve(Type type, params IOC.DependencyInstance[] dependencies)
        {
            var parameters = new List<AF.TypedParameter>();
            foreach (var dep in dependencies)
            {
                parameters.Add(new AF.TypedParameter(dep.Type, dep.Instance));
            }
            return _iocContext.Resolve(type, parameters);
        }
    }
}
