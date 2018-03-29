using System;
using System.Data;
using NSubstitute;
using Roham.Lib.Domain.Persistence;

namespace Roham.Lib.Domain.Test.Persistence
{
    public class GivenPersistenceUnitOfWorkFactory : UnitTestFixture
    {
        protected PersistenceUnitOfWorkFactory Subject;

        protected PersistenceUnitOfWorkFactory SetupSubject()
        {   
            var contextFactory = Substitute.For<IPersistenceContextFactory>();
            contextFactory
                .Create()
                .Returns<IPersistenceContext>(_ => Substitute.For<IPersistenceContext, IPersistenceContextExplicit>());

            return new PersistenceUnitOfWorkFactory(contextFactory);
            
        }

        protected PersistenceUnitOfWorkFactory SetupSubjectWithTransaction()
        {
            var contextFactory = Substitute.For<IPersistenceContextFactory>();

            contextFactory
                .Create()
                .Returns<IPersistenceContext>(_ => CreatePersistenceContextWithTransaction());

            return new PersistenceUnitOfWorkFactory(contextFactory);
        }

        private IPersistenceContext CreatePersistenceContextWithTransaction()
        {
            IPersistenceContext context = Substitute.For<IPersistenceContext, IPersistenceContextExplicit>();
            var contextExplict = context as IPersistenceContextExplicit;
            
            contextExplict
                .BeginTransaction(Arg.Any<IsolationLevel>())
                .Returns<IPersistenceTransaction>(_ => 
                {
                    var tranx = new MockPersistenceTransaction();                                                            
                    context
                        .IsInActiveTransaction
                        .Returns(__ => tranx.Status == PersistenceTransactionStatus.Active);

                    return tranx;
                });

            return context;
        }

        private class MockPersistenceTransaction : IPersistenceTransaction
        {
            public MockPersistenceTransaction()
            {
                Status = PersistenceTransactionStatus.Active;
            }

            public PersistenceTransactionStatus Status { get; private set; }

            public void Commit()
            {
                if (Status == PersistenceTransactionStatus.Active)
                {
                    Status = PersistenceTransactionStatus.Committed;
                }
            }

            public void Rollback()
            {
                if (Status == PersistenceTransactionStatus.Active)
                {
                    Status = PersistenceTransactionStatus.Rolledback;
                }
            }

            private bool _isDisposed = false;
            public void Dispose()
            {
                if (_isDisposed) return;

                if (Status == PersistenceTransactionStatus.Active)
                {
                    Status = PersistenceTransactionStatus.Invalid;
                }
                _isDisposed = true;
            }
        }
    }
}
