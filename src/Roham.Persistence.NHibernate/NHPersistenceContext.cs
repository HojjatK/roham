using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;
using RohamModel = Roham.Lib.Domain.Persistence;
using Roham.Data;
using Roham.Lib.Domain;
using Roham.Lib.Domain.Exceptions;

namespace Roham.Persistence.NHibernate
{
    internal class NHPersistenceContext : RohamModel.IPersistenceContext, RohamModel.IPersistenceContextExplicit
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ISession _session;

        public NHPersistenceContext(ISessionFactory sessionFactory, IDatabaseProvider dbProvider)
        {
            DatabaseProvider = dbProvider;
            _sessionFactory = sessionFactory;
            _session = _sessionFactory.GetCurrentSession();
        }

        internal ISession Session => _session;

        public IDatabaseProvider DatabaseProvider { get; }

        public IDbConnection Connection => _session?.Connection;

        public bool IsInActiveTransaction => _session?.Transaction != null && _session.Transaction.IsActive;

        RohamModel.IPersistenceTransaction RohamModel.IPersistenceContextExplicit.BeginTransaction(IsolationLevel isolationLevel)
        {
            var tx = _session.BeginTransaction(isolationLevel);
            if (tx == null)
            {
                throw new NullReferenceException("Transaction is null");
            }
            if (!tx.IsActive)
            {
                throw new InvalidOperationException("Transaction is not active");
            }
            return new NHGenericTransaction(tx);
        }

        public IQueryable<T> Query<T>() where T : Identifiable
        {
            return _session.Query<T>();
        }

        public IEnumerable<T> All<T>() where T : AggregateRoot
        {
            foreach(var entity in Query<T>().Where(e => true))
            {
                yield return entity;
            }
        }

        public IEnumerable<T> Where<T>(Func<T, bool> filter) where T : AggregateRoot
        {
            foreach (var entity in Query<T>().Where(filter))
            {
                yield return entity;
            }
        }

        public T FindById<T>(long id) where T : AggregateRoot
        {   
            var entity = _session.Get<T>(id);
            if (entity == null)
            {
                throw new EntityNotFoundException($"{typeof(T).Name} entity with id:{id} not found");
            }
            return entity;
        }

        public bool TryFindById<T>(long id, out T entity) where T : AggregateRoot
        {
            entity = _session.Get<T>(id);
            return entity != null;
        }

        public void Add<T>(T entity) where T : AggregateRoot
        {
            _session.Save(entity);
        }

        public void Update<T>(T entity) where T : AggregateRoot
        {
            _session.SaveOrUpdate(entity);
        }

        public void Remove<T>(T entity) where T : AggregateRoot
        {
            _session.Delete(entity);
        }

        void RohamModel.IPersistenceContextExplicit.Flush()
        {
            _session.Flush();
        }

        public void Clear()
        {
            _session.Clear();
        }

        private bool _isDisposed = false;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    var contextSession = CurrentSessionContext.Unbind(_sessionFactory);
                    if (contextSession != _session)
                    {
                        throw new Exception("Unbinded session is not the same as closing session");
                    }
                    if (_session.IsOpen)
                    {
                        try
                        {
                            if (_session.Transaction != null && _session.Transaction.IsActive)
                            {
                                // TODO: log a warning
                                _session.Transaction.Rollback();
                            }
                        }
                        finally
                        {
                            _session.Close();
                        }
                    }
                }
                finally
                {
                    if (_session != null)
                    {
                        try
                        {
                            if (_session.Transaction != null)
                            {
                                _session.Transaction.Dispose();
                            }
                        }
                        finally
                        {
                            _session.Dispose();
                        }
                    }
                }
            }
        }

        #region Nested Classes

        private class NHGenericTransaction : RohamModel.IPersistenceTransaction
        {
            private readonly ITransaction _transaction;

            public NHGenericTransaction(ITransaction transaction)
            {
                Objects.Requires(transaction != null, () => new NullReferenceException(nameof(ITransaction)));
                _transaction = transaction;
            }

            public RohamModel.PersistenceTransactionStatus Status
            {
                get
                {
                    if (_transaction.IsActive)
                    {
                        return RohamModel.PersistenceTransactionStatus.Active;
                    }
                    if (_transaction.WasCommitted)
                    {
                        return RohamModel.PersistenceTransactionStatus.Committed;
                    }
                    if (_transaction.WasRolledBack)
                    {
                        return RohamModel.PersistenceTransactionStatus.Rolledback;
                    }
                    return RohamModel.PersistenceTransactionStatus.Invalid;
                }
            }
            public bool IsActive => _transaction.IsActive;
            public bool WasCommitted => _transaction.WasCommitted;
            public bool WasRolledBack => _transaction.WasRolledBack;

            public void Commit()
            {
                if (_transaction.IsActive)
                {
                    _transaction.Commit();
                }
                else
                {
                    throw new InvalidOperationException("transaction is not active to commit");
                }
            }

            public void Rollback()
            {
                if (_transaction.IsActive)
                    _transaction.Rollback();
            }

            public void Dispose()
            {
                _transaction.Dispose();
            }
        }

        #endregion
    }
}