using Roham.Lib.Domain.Persistence;
using System.Threading;
using NUnit.Framework;
using NSubstitute;

namespace Roham.Lib.Domain.Test.Persistence
{
    public partial class PersistenceUnitOfWorkTest
    {
        [TestFixture]
        [Category("UnitTests.Domain.UnitOfWork")]
        internal class WhenUnitOfWorkUsedParallel : GivenPersistenceUnitOfWorkFactory
        {
            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                Subject = SetupSubject();
            }

            [Test]
            public void MultiThreads_Sync_UowCreation()
            {
                IPersistenceUnitOfWork uow1 = null, uow2 = null;

                Thread t = new Thread(new ThreadStart(() => {
                    using (uow2 = Subject.Create())
                    {
                        Thread.Sleep(500);
                        uow2.Complete();
                    }
                }));

                using (uow1 = Subject.Create())
                {
                    using (Subject.SuppressAmbientScope())
                    {
                        t.Start();
                    }
                    uow1.Complete();
                }

                t.Join();

                // assert
                IPersistenceContext context1 = uow1.Context;
                IPersistenceContext context2 = uow2.Context;

                Assert.AreNotSame(uow2, uow1);
                Assert.AreNotSame(context2, context1);

                (context1 as IPersistenceContextExplicit).Received(1).Flush();
                (context2 as IPersistenceContextExplicit).Received(1).Flush();
            }

            [Test]
            public void MultiThreads_Sync_InnerUowCreation()
            {
                IPersistenceUnitOfWork uow, innerUow, tUow = null, tInnerUow = null;

                Thread t = new Thread(new ThreadStart(() => {

                    using (tUow = Subject.Create())
                    {
                        using (tInnerUow = Subject.Create())
                        {
                            Thread.Sleep(500);
                            tInnerUow.Complete();
                        }
                        tUow.Complete();
                    }
                }));


                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.Create())
                    {
                        using (Subject.SuppressAmbientScope())
                        {
                            t.Start();
                        }
                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                t.Join();

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;
                IPersistenceContext tContext = tUow.Context;
                IPersistenceContext tChildContext = tInnerUow.Context;


                Assert.AreNotSame(uow, innerUow);
                Assert.AreSame(context, childContext);
                Assert.AreNotSame(uow, tUow);

                Assert.AreNotSame(tUow, tInnerUow);
                Assert.AreSame(tContext, tChildContext);
                Assert.AreNotSame(context, tContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
                (tContext as IPersistenceContextExplicit).Received(1).Flush();
            }
        }
    }
}
