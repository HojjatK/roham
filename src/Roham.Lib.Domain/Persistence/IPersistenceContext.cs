using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Roham.Data;

namespace Roham.Lib.Domain.Persistence
{
    public interface IPersistenceContext : IDisposable
    {
        IDbConnection Connection { get; }
        IDatabaseProvider DatabaseProvider { get; }

        IQueryable<T> Query<T>() where T : Identifiable;
        IEnumerable<T> All<T>() where T : AggregateRoot;
        IEnumerable<T> Where<T>(Func<T, bool> filter) where T : AggregateRoot;
        T FindById<T>(long id) where T : AggregateRoot;
        bool TryFindById<T>(long id, out T entity) where T : AggregateRoot;

        void Add<T>(T entity) where T : AggregateRoot;
        void Update<T>(T entity) where T : AggregateRoot;
        void Remove<T>(T entity) where T : AggregateRoot;

        bool IsInActiveTransaction { get; }
    }

    public interface IPersistenceContextExplicit
    {
        void Flush();        
        IPersistenceTransaction BeginTransaction(IsolationLevel isolationLevel);
    }

    public interface IPersistenceContextFactory
    {
        IPersistenceContext Create();
    }
}
