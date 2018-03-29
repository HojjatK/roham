using System;
using System.Threading.Tasks;
using System.Threading;

namespace Roham.Lib.Domain.Persistence
{
    public interface IPersistenceUnitOfWork : IDisposable
    {
        IPersistenceContext Context { get; }

        int Complete();
        Task<int> CompleteAsync();
        Task<int> CompleteAsync(CancellationToken cancelToken);
    }
}
