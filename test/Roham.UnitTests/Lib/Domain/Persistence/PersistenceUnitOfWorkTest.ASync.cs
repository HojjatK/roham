using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Roham.Lib.Domain.Persistence;

namespace Roham.Lib.Domain.Test.Persistence
{
    public partial class PersistenceUnitOfWorkTest
    {
        [TestFixture]
        [Category("UnitTests.Domain.UnitOfWork")]
        internal class WhenUnitOfWorkUsedAsync : GivenPersistenceUnitOfWorkFactory
        {
            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                Subject = SetupSubject();
            }


            [Test]
            public async void SingleThread_ASync_UowCreation()
            {
                IPersistenceUnitOfWork uow;

                Task t;
                using (uow = Subject.Create())
                {
                    t = uow.CompleteAsync();
                    await t;
                }
                t.Wait();

                // assert
                IPersistenceContext context = uow.Context;
                (context as IPersistenceContextExplicit).Received(1).Flush();
            }

            [Test]
            public async void SingleThread_ASync_MultipleUowsCreation()
            {
                Task t1;
                IPersistenceUnitOfWork uow1;
                using (uow1 = Subject.Create())
                {
                    t1 = uow1.CompleteAsync();
                    await t1;
                }

                Task t2;
                IPersistenceUnitOfWork uow2;
                using (uow2 = Subject.Create())
                {
                    t2 = uow2.CompleteAsync();
                    await t2;
                }

                t1.Wait();
                t2.Wait();

                // assert
                IPersistenceContext context1 = uow1.Context;
                IPersistenceContext context2 = uow2.Context;

                Assert.AreNotSame(uow2, uow1);
                Assert.AreNotSame(context2, context1);

                (context1 as IPersistenceContextExplicit).Received(1).Flush();
                (context2 as IPersistenceContextExplicit).Received(1).Flush();
            }

            [Test]
            public async void SingleThread_ASync_InnerUowCreation()
            {
                IPersistenceUnitOfWork uow, innerUow;
                Task t1, t2;
                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.Create())
                    {
                        t1 = innerUow.CompleteAsync();
                        await t1;
                    }
                    t2 = uow.CompleteAsync();
                    await t2;
                }

                t1.Wait();
                t2.Wait();
                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreSame(context, childContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
            }

            [Test]
            public async void SingleThread_ASync_InnerUowsCreation()
            {
                IPersistenceUnitOfWork uow, innerUow, innerInnerUow;
                Task t1, t2, t3;
                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.Create())
                    {
                        using (innerInnerUow = Subject.Create())
                        {
                            t1 = innerInnerUow.CompleteAsync();
                            await t1;
                        }
                        t2 = innerUow.CompleteAsync();
                        await t2;
                    }
                    t3 = uow.CompleteAsync();
                    await t3;
                }

                t1.Wait();
                t2.Wait();
                t3.Wait();

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;
                IPersistenceContext grandChildContext = innerInnerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreNotSame(uow, innerInnerUow);
                Assert.AreNotSame(innerUow, innerInnerUow);

                Assert.AreSame(context, childContext);
                Assert.AreSame(context, grandChildContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
            }

        }
    }
}
