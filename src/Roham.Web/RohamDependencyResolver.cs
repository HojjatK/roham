using System;
using IOC = Roham.Lib.Ioc;

namespace Roham.Web
{
    public class RohamDependencyResolver : IOC.AbstractDependencyResolver
    {
        protected RohamDependencyResolver(IOC.ILifetimeScope rootScope) : base(rootScope) { }

        private static RohamDependencyResolver _current;
        public static RohamDependencyResolver Current
        {
            get
            {
                if (_current == null)
                    throw new NullReferenceException($"{nameof(RohamDependencyResolver)} not initialized");
                return _current;
            }
        }

        public static void Initialize(IOC.ILifetimeScope rootScope)
        {
            if (_current == null)
            {
                _current = new RohamDependencyResolver(rootScope);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(RohamDependencyResolver)} already initialized");
            }
        }
    }
}
