using System;
using System.Threading.Tasks;
using System.Data;
using NUnit.Framework;
using NSubstitute;
using Roham.Lib.Domain.Persistence;

namespace Roham.Lib.Domain.Test.Persistence
{
    public partial class PersistenceUnitOfWorkTest
    {
        [TestFixture]
        [Category("UnitTests.Domain.UnitOfWork")]
        internal class WhenUnitOfWorkWithTransactionUsedAsync : GivenPersistenceUnitOfWorkFactory
        {
            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                Subject = SetupSubjectWithTransaction();
            }

            [Test]
            public async void SingleThread_ASync_UowWithTransactionCreation()
            {
                bool isActiveBeforeSave, isActiveAfterSave;
                IPersistenceUnitOfWork uow;

                Task t;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {

                    isActiveBeforeSave = uow.Context.IsInActiveTransaction;

                    t = uow.CompleteAsync();
                    await t;
                    isActiveAfterSave = uow.Context.IsInActiveTransaction;
                }

                t.Wait();
                // assert
                IPersistenceContext context = uow.Context;
                (context as IPersistenceContextExplicit).Received(1).Flush();


                Assert.IsTrue(isActiveBeforeSave);
                Assert.IsFalse(isActiveAfterSave);

                Assert.IsFalse(context.IsInActiveTransaction);
            }


            [Test]
            public async void SingleThread_ASync_MultipleUowsWithTransactionCreation()
            {
                IPersistenceUnitOfWork uow1;

                Task t1, t3;
                using (uow1 = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {
                    t1 = uow1.CompleteAsync();
                    await t1;
                }

                IPersistenceUnitOfWork uow2 = null;
                try
                {
                    using (uow2 = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                    {
                        ThrowsException();
                    }
                }
                catch (ApplicationException)
                {
                    // exception handled
                }

                IPersistenceUnitOfWork uow3;
                using (uow3 = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {
                    t3 = uow3.CompleteAsync();
                    await t3;
                }

                t1.Wait();
                t3.Wait();

                // assert
                IPersistenceContext context1 = uow1.Context;
                IPersistenceContext context2 = uow2.Context;
                IPersistenceContext context3 = uow3.Context;

                Assert.AreNotSame(uow2, uow1);
                Assert.AreNotSame(uow3, uow1);
                Assert.AreNotSame(uow3, uow2);
                Assert.AreNotSame(context2, context1);
                Assert.AreNotSame(context3, context1);
                Assert.AreNotSame(context3, context2);

                (context1 as IPersistenceContextExplicit).Received(1).Flush();
                Assert.IsFalse(context1.IsInActiveTransaction);

                (context2 as IPersistenceContextExplicit).Received(0).Flush();
                Assert.IsFalse(context2.IsInActiveTransaction);

                (context3 as IPersistenceContextExplicit).Received(1).Flush();
                Assert.IsFalse(context3.IsInActiveTransaction);
            }

            [Test]
            public async void SingleThread_ASync_InnerUowWihtTransactionCreation()
            {
                IPersistenceUnitOfWork uow, innerUow;

                Task t1, t2;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
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
            public async void SingleThread_ASync_InnerUowsWithTransactionCreation()
            {
                IPersistenceUnitOfWork uow, innerUow, innerInnerUow;

                Task t1, t2, t3;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
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
                Assert.IsFalse(context.IsInActiveTransaction);
            }


            private void ThrowsException()
            {
                throw new ApplicationException();
            }
        }
    }
}
