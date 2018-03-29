using System;
using NHibernate;
using RohamModel = Roham.Lib.Domain.Persistence;

namespace Roham.Persistence.NHibernate
{
    public abstract class NHibernateQueryHandler
    {
        private readonly RohamModel.IPersistenceContext _persistenceContext;

        protected NHibernateQueryHandler(RohamModel.IPersistenceContext persistenceContext)
        {
            _persistenceContext = persistenceContext;
        }

        protected ISession Session
        {
            get
            {
                var contextImpl = _persistenceContext as NHPersistenceContext;
                if (contextImpl != null)
                {
                    return contextImpl.Session;
                }
                throw new NullReferenceException("Current unit of work persistence context is null or invalid");
            }
        }

        protected bool IsFullTextEnabled(string tableName)
        {
            return _persistenceContext.DatabaseProvider.IsFullTextEnabled(Session.Connection, tableName);
        }
    }
}