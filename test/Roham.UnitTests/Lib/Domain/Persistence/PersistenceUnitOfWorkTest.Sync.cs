using NUnit.Framework;
using Roham.Lib.Domain.Persistence;
using NSubstitute;

namespace Roham.Lib.Domain.Test.Persistence
{
    public partial class PersistenceUnitOfWorkTest
    {
        [TestFixture]
        [Category("UnitTests.Domain.UnitOfWork")]
        internal class WhenPersistenceUnitOfWorkUsedSync : GivenPersistenceUnitOfWorkFactory
        {
            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                Subject = SetupSubject();
            }

            [Test]
            public void SingleThread_Sync_UowCreation()
            {
                IPersistenceUnitOfWork uow;

                using (uow = Subject.Create())
                {
                    uow.Complete();
                }

                // assert
                IPersistenceContextExplicit context = uow.Context as IPersistenceContextExplicit;
                context.Received(1).Flush();
            }

            [Test]
            public void SingleThread_Sync_MultipleUowsCreation()
            {
                IPersistenceUnitOfWork uow1;
                using (uow1 = Subject.Create())
                {
                    uow1.Complete();
                }

                IPersistenceUnitOfWork uow2;
                using (uow2 = Subject.Create())
                {
                    uow2.Complete();
                }


                // assert
                IPersistenceContextExplicit context1 = uow1.Context as IPersistenceContextExplicit;
                IPersistenceContextExplicit context2 = uow2.Context as IPersistenceContextExplicit;

                Assert.AreNotSame(uow2, uow1);
                Assert.AreNotSame(context2, context1);

                context1.Received(1).Flush();
                context2.Received(1).Flush();
            }

            [Test]
            public void SingleThread_Sync_InnerUowCreation()
            {
                IPersistenceUnitOfWork uow, innerUow;
                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.Create())
                    {
                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                // assert
                IPersistenceContextExplicit context = uow.Context as IPersistenceContextExplicit;
                IPersistenceContext childContext = innerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreSame(context, childContext);

                context.Received(1).Flush();
            }

            [Test]
            public void SingleThread_Sync_InnerUowsCreation()
            {
                IPersistenceUnitOfWork uow, innerUow, innerInnerUow;
                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.Create())
                    {
                        using (innerInnerUow = Subject.Create())
                        {
                            innerInnerUow.Complete();
                        }
                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                // assert
                IPersistenceContextExplicit context = uow.Context as IPersistenceContextExplicit;
                IPersistenceContext childContext = innerUow.Context;
                IPersistenceContext grandChildContext = innerInnerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreNotSame(uow, innerInnerUow);
                Assert.AreNotSame(innerUow, innerInnerUow);

                Assert.AreSame(context, childContext);
                Assert.AreSame(context, grandChildContext);

                context.Received(1).Flush();
            }
        }
    }
}
