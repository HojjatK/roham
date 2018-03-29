using System;
using NHibernate;
using NHibernate.Cfg;

namespace Roham.Persistence.NHibernate
{
    internal class CfgSessionFactory
    {
        public CfgSessionFactory(Configuration cfg, ISessionFactory factory)
        {
            Objects.Requires(cfg != null,() => new NullReferenceException(nameof(Configuration)));
            Objects.Requires(factory != null, () => new NullReferenceException(nameof(ISessionFactory)));

            Configuration = cfg;
            SessionFactory = factory;
        }

        public Configuration Configuration { get; private set; }
        public ISessionFactory SessionFactory { get; private set; }
    }
}
