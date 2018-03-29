using System;
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
        internal class WhenPersistenceUnitOfWorkWithTransactionUsedSync : GivenPersistenceUnitOfWorkFactory
        {
            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                Subject = SetupSubjectWithTransaction();
            }

            [Test]
            public void SingleThread_Sync_UowWithTransactionCreation()
            {
                bool isActiveBeforeSave, isActiveAfterSave;
                IPersistenceUnitOfWork uow;

                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {

                    isActiveBeforeSave = uow.Context.IsInActiveTransaction;

                    uow.Complete();
                    isActiveAfterSave = uow.Context.IsInActiveTransaction;
                }

                // assert
                IPersistenceContextExplicit context = uow.Context as IPersistenceContextExplicit;
                context.Received(1).Flush();


                Assert.IsTrue(isActiveBeforeSave);
                Assert.IsFalse(isActiveAfterSave);
            }

            [Test]
            public void SingleThread_Sync_UowWithRollbackTransactionCreation()
            {
                IPersistenceUnitOfWork uow = null;

                try
                {
                    using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                    {
                        ThrowsException();
                    }
                }
                catch (ApplicationException)
                {
                    // exception handled
                }

                // assert
                IPersistenceContext context = uow.Context;
                (context as IPersistenceContextExplicit).Received(0).Flush();
                Assert.IsFalse(context.IsInActiveTransaction);
            }

            [Test]
            public void SingleThread_Sync_MultipleUowsWithTransactionCreation()
            {
                IPersistenceUnitOfWork uow1;
                using (uow1 = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {
                    uow1.Complete();
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
                    uow3.Complete();
                }


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
            public void SingleThread_Sync_InnerUowWihtTransactionCreation_1()
            {
                IPersistenceUnitOfWork uow, innerUow;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {
                    using (innerUow = Subject.Create())
                    {
                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreSame(context, childContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
            }

            [Test]
            public void SingleThread_Sync_InnerUowWihtTransactionCreation_2()
            {
                IPersistenceUnitOfWork uow, innerUow;
                using (uow = Subject.Create())
                {
                    using (innerUow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                    {
                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreNotSame(context, childContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
                Assert.IsFalse(context.IsInActiveTransaction);

                (childContext as IPersistenceContextExplicit).Received(1).Flush();
                Assert.IsFalse(childContext.IsInActiveTransaction);
            }

            [Test]
            public void SingleThread_Sync_InnerUowsWithTransactionCreation_1()
            {
                IPersistenceUnitOfWork uow, innerUow, innerInnerUow;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
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

            [Test]
            public void SingleThread_Sync_InnerUowsWithTransactionCreation_2()
            {
                IPersistenceUnitOfWork uow, innerUow, innerInnerUow = null;
                using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                {
                    using (innerUow = Subject.Create())
                    {
                        try
                        {
                            using (innerInnerUow = Subject.CreateWithTransaction(IsolationLevel.RepeatableRead))
                            {
                                ThrowsException();
                            }
                        }
                        catch (ApplicationException)
                        {
                            // exception handled
                        }

                        innerUow.Complete();
                    }
                    uow.Complete();
                }

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;
                IPersistenceContext grandChildContext = innerInnerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreNotSame(uow, innerInnerUow);
                Assert.AreNotSame(innerUow, innerInnerUow);

                Assert.AreSame(context, childContext);
                Assert.AreNotSame(context, grandChildContext);

                (context as IPersistenceContextExplicit).Received(1).Flush();
                Assert.IsFalse(context.IsInActiveTransaction);

                (grandChildContext as IPersistenceContextExplicit).Received(0).Flush();
                Assert.IsFalse(grandChildContext.IsInActiveTransaction);
            }

            [Test]
            public void SingleThread_Sync_InnerUowsWithTransactionCreation_3()
            {
                IPersistenceUnitOfWork uow = null, innerUow = null, grandInnerUow = null;
                try
                {
                    using (uow = Subject.CreateWithTransaction(IsolationLevel.ReadCommitted))
                    {
                        using (innerUow = Subject.Create())
                        {
                            using (grandInnerUow = Subject.CreateWithTransaction(IsolationLevel.RepeatableRead))
                            {
                                ThrowsException();
                            }
                            innerUow.Complete();
                        }
                        uow.Complete();
                    }
                }
                catch (ApplicationException)
                {
                    // exception handled
                }

                // assert
                IPersistenceContext context = uow.Context;
                IPersistenceContext childContext = innerUow.Context;
                IPersistenceContext grandChildContext = grandInnerUow.Context;

                Assert.AreNotSame(uow, innerUow);
                Assert.AreNotSame(uow, grandInnerUow);
                Assert.AreNotSame(innerUow, grandInnerUow);

                Assert.AreSame(context, childContext);
                Assert.AreNotSame(context, grandChildContext);

                (context as IPersistenceContextExplicit).Received(0).Flush();
                Assert.IsFalse(context.IsInActiveTransaction);

                (grandChildContext as IPersistenceContextExplicit).Received(0).Flush();
                Assert.IsFalse(grandChildContext.IsInActiveTransaction);
            }

            private void ThrowsException()
            {
                throw new ApplicationException();
            }
        }
    }
}
