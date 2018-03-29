using System;
using System.Collections;
using NUnit.Framework;
using Roham.Lib.Domain;
using Roham.Persistence.NHibernate.Configurators;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace Roham.Persistence.NHibernate
{
    public abstract class NHibernateFixture 
    {
        private static CfgSessionFactory _configuredSessionFactory;
        private static SqliteMemoryPersistenceProviderConfigurator _sqliteInMemoryDatabaseProvider =
            new SqliteMemoryPersistenceProviderConfigurator(
                typeof(Roham.Domain.Entities.Security.UserMapping).Assembly,
                new NHFixturePersistenceConfiguration { AdoNetBatchSize = 200, ShowSql = true });
        private bool _dbCanBeDirty;

        protected NHibernateFixture(bool dbCanBeDirty)
        {
            _dbCanBeDirty = dbCanBeDirty;
        }

        private static CfgSessionFactory ConfiguredSessionFactory
        {
            get
            {
                if (_configuredSessionFactory == null)
                {
                    _configuredSessionFactory =
                        (_sqliteInMemoryDatabaseProvider as INHPersistenceProviderConfigurator).CreateSessionFactory("");
                }
                return _configuredSessionFactory;
            }
        }

        protected Configuration Configuration
        {
            get
            {
                return ConfiguredSessionFactory.Configuration;
            }
        }

        protected ISessionFactory SessionFactory
        {
            get
            {
                return ConfiguredSessionFactory.SessionFactory;
            }
        }

        protected ISession Session
        {
            get
            {
                return SessionFactory.GetCurrentSession();
            }
        }

        private static bool _dbExists = false;
        protected bool DoesDatabaseExist()
        {
            return _dbExists;
        }

        protected void CreateDatabase()
        {
            if (_dbExists)
            {
                DropDatabase();
            }
            new SchemaUpdate(Configuration).Execute(false, true);
            _dbExists = true;
        }

        protected void DropDatabase()
        {
            _sqliteInMemoryDatabaseProvider.CloseDatabase();
            _dbExists = false;
        }

        protected void SetupNHibernateSession()
        {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
        }

        protected void TeardownNHibernateSession()
        {
            // Teardown contextual session
            if (Session != null)
            {
                var session = CurrentSessionContext.Unbind(SessionFactory);
                session.Dispose();
            }
        }


        [TestFixtureSetUp]
        public void OnFixtureSetup()
        {
            if (!_dbCanBeDirty || !DoesDatabaseExist())
            {
                CreateDatabase();
            }
        }

        [TestFixtureTearDown]
        public void OnFixtureTeardown()
        {
            if (!_dbCanBeDirty)
            {
                DropDatabase();
            }
        }

        [SetUp]
        public void OnSetup()
        {
            if (!_dbCanBeDirty)
            {
                CreateDatabase();
            }
            SetupNHibernateSession();
        }

        [TearDown]
        public void OnTeardown()
        {
            TeardownNHibernateSession();
            if (!_dbCanBeDirty)
            {
                DropDatabase();
            }
        }

        protected class CustomEqualityComparer : IEqualityComparer
        {
            public new bool Equals(object x, object y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (x == null || y == null)
                    return false;
                if (x is Identifiable && y is Identifiable)
                {
                    return (x as Identifiable).Id == (y as Identifiable).Id;
                }
                if (x is DateTime && y is DateTime)
                {
                    return string.Format("{0:ddMMyyyyHHmmss}", x) == string.Format("{0:ddMMyyyyHHmmss}", y);
                }
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
