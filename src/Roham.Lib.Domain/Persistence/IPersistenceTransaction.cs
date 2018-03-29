using System;

namespace Roham.Lib.Domain.Persistence
{
    public enum PersistenceTransactionStatus
    {
        NotStarted,
        Active,
        Committed,
        Rolledback,
        Invalid
    }

    public interface IPersistenceTransaction : IDisposable
    {
        PersistenceTransactionStatus Status { get; }
        void Commit();
        void Rollback();
    }
}
