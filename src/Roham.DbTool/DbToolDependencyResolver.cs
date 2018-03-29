using System;
using Roham.Lib.Ioc;

namespace Roham.DbTool
{
    public class DbToolDependencyResolver : AbstractDependencyResolver
    {
        protected DbToolDependencyResolver(ILifetimeScope rootScope) : base(rootScope) { }

        private static DbToolDependencyResolver _current;
        public static DbToolDependencyResolver Current
        {
            get
            {
                if (_current == null)
                    throw new NullReferenceException($"{nameof(DbToolDependencyResolver)} not initialized");
                return _current;
            }
        }

        public static void Initialize(ILifetimeScope rootScope)
        {
            if (_current == null)
            {
                _current = new DbToolDependencyResolver(rootScope);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(DbToolDependencyResolver)} already initialized");
            }
        }
    }
}
