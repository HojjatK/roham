using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Roham.Lib.Domain.Persistence
{
    public enum PersistenceUnitOfWorkOption
    {
        /// <summary>
        /// Join the ambient scope if one exists. Creates a new one otherwise.
        /// </summary>
        JoinExisting,

        /// <summary>
        /// Ignore the ambient scope (if any) and force the creation of a new scope. 
        /// 
        /// You would typically do this to ensure that the changes made within the inner scope
        /// are always persisted regardless of the outcome of the overall business transaction
        /// (e.g. to persist the results of an operation, such as a remote API call, that
        /// cannot be rolled back or to persist audit or log entries that must not be rolled back
        /// regardless of the outcome of the business transaction). 
        /// </summary>
        ForceCreateNew
    }

    public interface IPersistenceUnitOfWorkFactory
    {
        IPersistenceUnitOfWork CreateReadOnly();

        IPersistenceUnitOfWork Create(
            PersistenceUnitOfWorkOption joiningOption = PersistenceUnitOfWorkOption.JoinExisting,
            bool readOnly = false);

        IPersistenceUnitOfWork CreateWithTransaction(IsolationLevel isolationLevel);

        IDisposable SuppressAmbientScope();
    }

    public class PersistenceUnitOfWorkFactory : IPersistenceUnitOfWorkFactory
    {
        private readonly IPersistenceContextFactory _persistenceContextFactory;

        public PersistenceUnitOfWorkFactory(IPersistenceContextFactory persistenceContextFactory)
        {
            _persistenceContextFactory = persistenceContextFactory;
        }

        public IPersistenceUnitOfWork CreateReadOnly()
        {
            return new PersistenceScopeImpl(
                _persistenceContextFactory,
                PersistenceUnitOfWorkOption.JoinExisting,
                readOnly: true,
                isolationLevel: null);
        }

        public IPersistenceUnitOfWork Create(
            PersistenceUnitOfWorkOption joiningOption = PersistenceUnitOfWorkOption.JoinExisting, 
            bool readOnly = false)
        {
            return new PersistenceScopeImpl(
                _persistenceContextFactory,
                joiningOption,
                readOnly: readOnly,
                isolationLevel: null);
        }

        public IPersistenceUnitOfWork CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new PersistenceScopeImpl(
                _persistenceContextFactory,
                PersistenceUnitOfWorkOption.ForceCreateNew,
                readOnly: false,
                isolationLevel: isolationLevel);
        }

        public IDisposable SuppressAmbientScope()
        {
            return new AmbientScopeSuppressor();
        }

        #region Nested Classes

        private class PersistenceScopeImpl : IPersistenceUnitOfWork
        {
            private readonly IPersistenceContextFactory _persistenceContextFactory;
            private bool _disposed;
            private bool _readOnly;
            private bool _completed;
            private bool _nested;
            private readonly PersistenceScopeImpl _parentScope;
            private readonly IPersistenceContext _persistenceContext;
            private readonly IPersistenceTransaction _persistenceTransaction;

            public PersistenceScopeImpl(IPersistenceContextFactory persistenceContextFactory) :
                this(persistenceContextFactory,
                    PersistenceUnitOfWorkOption.JoinExisting,
                    readOnly: false,
                    isolationLevel: null)
            { }

            public PersistenceScopeImpl(
                IPersistenceContextFactory persistenceContextFactory,
                PersistenceUnitOfWorkOption joiningOption,
                bool readOnly,
                IsolationLevel? isolationLevel)
            {
                _persistenceContextFactory = persistenceContextFactory;
                if (isolationLevel.HasValue &&
                    joiningOption == PersistenceUnitOfWorkOption.JoinExisting)
                {
                    throw new ArgumentException(
@"Cannot join an ambient persistence scope when an explicit database transaction is required. 
When requiring explicit database transactions to be used (i.e. when the 'isolationLevel' parameter is set), 
you must not also ask to join the ambient scope (i.e. the 'joinAmbient' parameter must be set to false).");
                }

                _disposed = false;
                _completed = false;
                _readOnly = readOnly;
                _parentScope = GetAmbientScope();

                if (_parentScope != null && joiningOption == PersistenceUnitOfWorkOption.JoinExisting)
                {
                    if (_parentScope._readOnly && !this._readOnly)
                    {
                        throw new InvalidOperationException("Cannot nest a read/write Scope within a read-only Scope.");
                    }
                    _nested = true;
                    _persistenceContext = _parentScope._persistenceContext;

                    if (_persistenceContext as IPersistenceContextExplicit == null)
                    { 
                        throw new InvalidProgramException($"Parent PersistenceContext does not implement {typeof(IPersistenceContextExplicit).Name}");
                    }
                }
                else
                {
                    _nested = false;
                    _persistenceContext = _persistenceContextFactory.Create();

                    var explicitContext = _persistenceContext as IPersistenceContextExplicit;
                    if (explicitContext == null)
                    {
                        throw new InvalidProgramException($"PersistenceContext does not implement {typeof(IPersistenceContextExplicit).Name}");
                    }

                    if (isolationLevel.HasValue)
                    {
                        _persistenceTransaction = explicitContext.BeginTransaction(isolationLevel.Value);
                    }
                }
                SetAmbientScope(this);

            }

            public IPersistenceContext Context
            {
                get { return _persistenceContext; }
            }

            public int Complete()
            {
                ValidateNotCompleted();

                // Only save changes if we're not a nested scope. 
                // Otherwise, let the top-level scope decide when the changes should be saved.
                var c = 0;
                if (!_nested)
                {
                    Commit();
                    c = 1;
                }
                _completed = true;

                return c;
            }

            public Task<int> CompleteAsync()
            {
                return CompleteAsync(CancellationToken.None);
            }

            public async Task<int> CompleteAsync(CancellationToken cancelToken)
            {
                if (cancelToken == null)
                {
                    throw new ArgumentNullException("cancelToken");
                }
                ValidateNotCompleted();

                // Only save changes if we're not a nested scope. Otherwise, let the top-level scope 
                // decide when the changes should be saved.
                var c = 0;
                if (!_nested)
                {
                    c = await CommitAsync(cancelToken).ConfigureAwait(false);
                }

                _completed = true;
                return c;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                // Commit/Rollback and dispose PersistenceContext instance
                if (!_nested)
                {
                    if (!_completed)
                    {
                        // Do our best to clean up as much as we can but don't throw exception here as it's too late anyway.
                        try
                        {
                            if (_readOnly)
                            {
                                // Disposing a read-only scope before having called its SaveChanges() method
                                // is the normal and expected behavior. Read-only scopes get committed automatically.
                                Commit();
                            }
                            else
                            {
                                // Disposing a read/write scope before having called its SaveChanges() method
                                // indicates that something went wrong and that all changes should be rolled-back.
                                RollbackTransaction();
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                        }

                        _completed = true;
                    }

                    _persistenceContext.Dispose();
                }

                // Pop ourself from the ambient scope stack
                var currentAmbientScope = GetAmbientScope();
                if (currentAmbientScope != this)
                {
                    // This is a serious programming error. Worth throwing here.
                    throw new InvalidOperationException("PersistenceScope instances must be disposed of in the order in which they were created!");
                }

                RemoveAmbientUnitOfWork();

                if (_parentScope != null)
                {
                    if (_parentScope._disposed)
                    {
                        /*
                         * If our parent scope has been disposed before us, it can only mean one thing:
                         * someone started a parallel flow of execution and forgot to suppress the
                         * ambient context before doing so. And we've been created in that parallel flow.
                         * 
                         * Since the CallContext flows through all async points, the ambient scope in the 
                         * main flow of execution ended up becoming the ambient scope in this parallel flow
                         * of execution as well. So when we were created, we captured it as our "parent scope". 
                         * 
                         * The main flow of execution then completed while our flow was still ongoing. When 
                         * the main flow of execution completed, the ambient scope there (which we think is our 
                         * parent scope) got disposed of as it should.
                         * 
                         * So here we are: our parent scope isn't actually our parent scope. It was the ambient
                         * scope in the main flow of execution from which we branched off. We should never have seen 
                         * it. Whoever wrote the code that created this parallel task should have suppressed
                         * the ambient context before creating the task - that way we wouldn't have captured
                         * this bogus parent scope.
                         * 
                         * While this is definitely a programming error, it's not worth throwing here. We can only 
                         * be in one of two scenario:
                         * 
                         * - If the developer who created the parallel task was mindful to force the creation of 
                         * a new scope in the parallel task (with UnitOfWorkFactory.CreateNew() instead of 
                         * JoinOrCreate()) then no harm has been done. We haven't tried to access the same DbContext
                         * instance from multiple threads.
                         * 
                         * - If this was not the case, they probably already got an exception complaining about the same
                         * DbContext or ObjectContext being accessed from multiple threads simultaneously (or a related
                         * error like multiple active result sets on a DataReader, which is caused by attempting to execute
                         * several queries in parallel on the same DbContext instance). So the code has already blow up.
                         * 
                         * So just record a warning here. Hopefully someone will see it and will fix the code.
                         * 
                         */
                        var message =
@"PROGRAMMING ERROR - When attempting to dispose a PersistenceScope, we found that our parent PersistenceScope has already been disposed! 
This means that someone started a parallel flow of execution (e.g. created a TPL task, created a thread or enqueued a work item on the ThreadPool) 
within the context of a PersistenceScope without suppressing the ambient context first. 

In order to fix this:
1) Look at the stack trace below - this is the stack trace of the parallel task in question.
2) Find out where this parallel task was created.
3) Change the code so that the ambient context is suppressed before the parallel task is created. 
   You can do this with PersistenceScopeFactory.SuppressAmbientScope() (wrap the parallel task creation code block in this). 

Stack Trace:
" + Environment.StackTrace;

                        System.Diagnostics.Debug.WriteLine(message);
                    }
                    else
                    {
                        SetAmbientScope(_parentScope);
                    }
                }

                _disposed = true;
            }

            private void ValidateNotCompleted()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("PersistenceScope");
                }
                if (_completed)
                {
                    throw new InvalidOperationException(
@"You cannot call SaveChanges() more than once on a PersistenceScope. A PersistenceScope is meant to encapsulate a business transaction: 
create the scope at the start of the business transaction and then call SaveChanges() at the end. Calling SaveChanges() mid-way through 
a business transaction doesn't make sense and most likely mean that you should refactor your service method into two separate service method 
that each create their own PersistenceScope and each implement a single business transaction.");
                }
            }

            private void Commit()
            {
                try
                {
                    if (!_readOnly)
                    {
                        var explicitContext = _persistenceContext as IPersistenceContextExplicit;
                        explicitContext.Flush();
                    }

                    // If we've started an explicit database transaction, time to commit it now.
                    CommitTransactionIfAny();
                }
                finally
                {
                    _completed = true;
                }
            }

            private async Task<int> CommitAsync(CancellationToken cancelToken)
            {
                try
                {
                    if (!_readOnly)
                    {
                        await Task.Run(() =>
                        {
                            var explicitContext = _persistenceContext as IPersistenceContextExplicit;
                            explicitContext.Flush();

                            // If we've started an explicit database transaction, time to commit it now.
                            CommitTransactionIfAny();
                        }, cancelToken)
                        .ConfigureAwait(false);
                    }

                }
                finally
                {
                    _completed = true;
                }
                return 1;
            }

            private void CommitTransactionIfAny()
            {
                var tran = _persistenceTransaction;
                if (tran != null)
                {
                    tran.Commit();
                    tran.Dispose();
                }
            }

            private void RollbackTransaction()
            {
                var tran = _persistenceTransaction;
                if (tran != null)
                {
                    tran.Rollback();
                    tran.Dispose();
                }
            }

            #region Ambient Scope Logic

            /*
            * This is where all the magic happens. And there is not much of it.
            * 
            * This implementation is inspired by the source code of the
            * TransactionScope class in .NET 4.5.1 (the TransactionScope class
            * is prior versions of the .NET Fx didn't have support for async
            * operations).
            * 
            * In order to understand this, you'll need to be familiar with the
            * concept of async points. You'll also need to be familiar with the
            * ExecutionContext and CallContext and understand how and why they 
            * flow through async points. Stephen Toub has written an
            * excellent blog post about this - it's a highly recommended read:
            * http://blogs.msdn.com/b/pfxteam/archive/2012/06/15/executioncontext-vs-synchronizationcontext.aspx
            * 
            * Overview: 
            * 
            * We want our PersistenceContextScope instances to be ambient within 
            * the context of a logical flow of execution. This flow may be 
            * synchronous or it may be asynchronous.
            * 
            * If we only wanted to support the synchronous flow scenario, 
            * we could just store our PersistenceContextScope instances in a ThreadStatic 
            * variable. That's the "traditional" (i.e. pre-async) way of implementing
            * an ambient context in .NET. You can see an example implementation of 
            * a TheadStatic-based ambient PersistenceContext here: http://coding.abel.nu/2012/10/make-the-dbcontext-ambient-with-unitofworkscope/ 
            * 
            * But that would be hugely limiting as it would prevent us from being
            * able to use the new async features added to Entity Framework
            * in EF6 and .NET 4.5.
            * 
            * So we need a storage place for our PersistenceContextScope instances 
            * that can flow through async points so that the ambient context is still 
            * available after an await (or any other async point). And this is exactly 
            * what CallContext is for.
            * 
            * There are however two issues with storing our PersistenceContextScope instances 
            * in the CallContext:
            * 
            * 1) Items stored in the CallContext should be serializable. That's because
            * the CallContext flows not just through async points but also through app domain 
            * boundaries. I.e. if you make a remoting call into another app domain, the
            * CallContext will flow through this call (which will require all the values it
            * stores to get serialized) and get restored in the other app domain.
            * 
            * In our case, our PersistenceContextScope instances aren't serializable. And in any case,
            * we most definitely don't want them to be flown accross app domains. So we'll
            * use the trick used by the TransactionScope class to work around this issue.
            * Instead of storing our PersistenceContextScope instances themselves in the CallContext,
            * we'll just generate a unique key for each instance and only store that key in 
            * the CallContext. We'll then store the actual PersistenceContextScope instances in a static
            * Dictionary against their key. 
            * 
            * That way, if an app domain boundary is crossed, the keys will be flown accross
            * but not the PersistenceContextScope instances since a static variable is stored at the 
            * app domain level. The code executing in the other app domain won't see the ambient
            * PersistenceContextScope created in the first app domain and will therefore be able to create
            * their own ambient DbContextScope if necessary.
            * 
            * 2) The CallContext is flow through *all* async points. This means that if someone
            * decides to create multiple threads within the scope of a PersistenceContextScope, our ambient scope
            * will flow through all the threads. Which means that all the threads will see that single 
            * PersistenceContextScope instance as being their ambient DbContext. So clients need to be 
            * careful to always suppress the ambient context before kicking off a parallel operation
            * to avoid our PersistenceContextScope instances from being accessed from multiple threads.
            * 
            */
            private static readonly string AmbientScopeKey = "AmbientScope_" + Guid.NewGuid();

            // Use a ConditionalWeakTable instead of a simple ConcurrentDictionary to store our PersistenceContextScope instances 
            // in order to prevent leaking PersistenceContextScope instances if someone doesn't dispose them properly.
            //
            // For example, if we used a ConcurrentDictionary and someone let go of a PersistenceContextScope instance without 
            // disposing it, our ConcurrentDictionary would still have a reference to it, preventing
            // the GC from being able to collect it => leak. With a ConditionalWeakTable, we don't hold a reference
            // to the PersistenceContextScope instances we store in there, allowing them to get GCed.
            // The doc for ConditionalWeakTable isn't the best. This SO anser does a good job at explaining what 
            // it does: http://stackoverflow.com/a/18613811
            private static readonly ConditionalWeakTable<InstanceIdentifier, PersistenceScopeImpl> ScopeInstances = new ConditionalWeakTable<InstanceIdentifier, PersistenceScopeImpl>();

            private InstanceIdentifier _instanceIdentifier = new InstanceIdentifier();

            /// <summary>
            /// Get the current ambient scope or null if no ambient scope has been setup.
            /// </summary>
            internal static PersistenceScopeImpl GetAmbientScope()
            {
                // Retrieve the identifier of the ambient scope (if any)
                var instanceIdentifier = CallContext.LogicalGetData(AmbientScopeKey) as InstanceIdentifier;
                if (instanceIdentifier == null)
                {
                    return null; // Either no ambient scope has been set or we've crossed an app domain boundary and have (intentionally) lost the ambient scope
                }

                // Retrieve the PersistenceContextScope instance corresponding to this identifier
                PersistenceScopeImpl ambientScope;
                if (ScopeInstances.TryGetValue(instanceIdentifier, out ambientScope))
                {
                    return ambientScope;
                }

                // We have an instance identifier in the CallContext but no corresponding instance
                // in our PersistenceContextScopeInstances table. This should never happen! The only place where
                // we remove the instance from the PersistenceContextScopeInstances table is in RemoveAmbientScope(),
                // which also removes the instance identifier from the CallContext. 
                //
                // There's only one scenario where this could happen: someone let go of a PersistenceContextScope 
                // instance without disposing it. In that case, the CallContext
                // would still contain a reference to the scope and we'd still have that scope's instance
                // in our PersistenceContextScopeInstances table. But since we use a ConditionalWeakTable to store 
                // our DbContextScope instances and are therefore only holding a weak reference to these instances, 
                // the GC would be able to collect it. Once collected by the GC, our ConditionalWeakTable will return
                // null when queried for that instance. In that case, we're OK. This is a programming error 
                // but our use of a ConditionalWeakTable prevented a leak.
                System.Diagnostics.Debug.WriteLine(
@"Programming error detected. Found a reference to an ambient PersistenceContextScope in 
the CallContext but didn't have an instance for it in our PersistenceContextScopeInstances table. 
This most likely means that this PersistenceContextScope instance wasn't disposed of properly. 
PersistenceContextScope instance must always be disposed. Review the code for any PersistenceContextScope 
instance used outside of a 'using' block and fix it so that all PersistenceContextScope instances are disposed of.");

                return null;
            }

            /// <summary>
            /// Makes the provided 'PersistenceContextScope' available as the the ambient scope via the CallContext.
            /// </summary>
            internal static void SetAmbientScope(PersistenceScopeImpl newScope)
            {
                if (newScope == null)
                {
                    throw new ArgumentNullException("newAmbientScope");
                }

                var current = CallContext.LogicalGetData(AmbientScopeKey) as InstanceIdentifier;

                if (current == newScope._instanceIdentifier)
                {
                    return;
                }

                // Store the new scope's instance identifier in the CallContext, making it the ambient scope
                CallContext.LogicalSetData(AmbientScopeKey, newScope._instanceIdentifier);

                // Keep track of this instance (or do nothing if we're already tracking it)
                ScopeInstances.GetValue(newScope._instanceIdentifier, key => newScope);
            }

            /// <summary>
            /// Clears the ambient scope from the CallContext and stops tracking its instance. 
            /// Call this when a DbContextScope is being disposed.
            /// </summary>
            internal static void RemoveAmbientUnitOfWork()
            {
                var current = CallContext.LogicalGetData(AmbientScopeKey) as InstanceIdentifier;
                CallContext.LogicalSetData(AmbientScopeKey, null);

                // If there was an ambient scope, we can stop tracking it now
                if (current != null)
                {
                    ScopeInstances.Remove(current);
                }
            }

            /// <summary>
            /// Clears the ambient scope from the CallContext but keeps tracking its instance. Call this to temporarily 
            /// hide the ambient context (e.g. to prevent it from being captured by parallel task).
            /// </summary>
            internal static void HideAmbientUnitOfWork()
            {
                CallContext.LogicalSetData(AmbientScopeKey, null);
            }

            #endregion
        }

        /*
           * The idea of using an object reference as our instance identifier 
           * instead of simply using a unique string (which we could have generated
           * with Guid.NewGuid() for example) comes from the TransactionScope
           * class. As far as I can make out, a string would have worked just fine.
           * I'm guessing that this is done for optimization purposes. Creating
           * an empty class is cheaper and uses up less memory than generating
           * a unique string.
           */
        internal class InstanceIdentifier : MarshalByRefObject { }

        private class AmbientScopeSuppressor : IDisposable
        {
            PersistenceScopeImpl _savedScope;
            private bool _disposed;

            public AmbientScopeSuppressor()
            {
                _savedScope = PersistenceScopeImpl.GetAmbientScope();

                // We're hiding the ambient scope but not removing its instance
                // altogether. This is to be tolerant to some programming errors. 
                // 
                // Suppose we removed the ambient scope instance here. If someone
                // was to start a parallel task without suppressing
                // the ambient context and then tried to suppress the ambient
                // context within the parallel task while the original flow
                // of execution was still ongoing (a strange thing to do, I know,
                // but I'm sure this is going to happen), we would end up 
                // removing the ambient context instance of the original flow 
                // of execution from within the parallel flow of execution!
                // 
                // As a result, any code in the original flow of execution
                // that would attempt to access the ambient scope would end up 
                // with a null value since we removed the instance.
                //
                // It would be a fairly nasty bug to track down. So don't let
                // that happen. Hiding the ambient scope (i.e. clearing the CallContext
                // in our execution flow but leaving the ambient scope instance untouched)
                // is safe.            
                PersistenceScopeImpl.HideAmbientUnitOfWork();
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                if (_savedScope != null)
                {
                    PersistenceScopeImpl.SetAmbientScope(_savedScope);
                    _savedScope = null;
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
