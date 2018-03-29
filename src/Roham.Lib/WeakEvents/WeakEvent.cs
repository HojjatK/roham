using System;
using System.Linq;

namespace Roham.Lib.WeakEvents
{
    /// <summary>
    /// Used to allow registration of event handlers with weak reference to event handler target instance.
    /// When the target instance goes out of scope and garbage collector finally collects it, the event handler is automatically unregistered.
    /// This solves a general problem with current .NET event registration mechanism where a strong reference to the caller is created, thus
    /// preventing the target object from being garbage collected event when it goes out of scope. This solution uses a <see cref="WeakReference" />
    /// to hold on to the target, thus preventing it from leaking.
    /// 
    /// <example>
    /// Assume we wish to declare an event with delegate signature
    /// 
    /// public delegate MyEvent(int pNum, Entity pEntity)
    /// 
    /// To declare a weak event follow the steps below:
    /// first declare the private delegate that will maintain callback references, followed by event declaration for add/remove
    /// 
    /// private WeakEvent<MyEvent> mMyEventHandlers = new WeakEvent<MyEvent>((e)=>e.h);
    /// public event MyEvent OnMyEvent
    /// {
    ///     add
    ///     {
    ///         mMyEventHandlers += value;
    ///     }
    ///     remove
    ///     {
    ///         mMyEventHandlers -= value;
    ///     }
    /// }
    ///
    /// </example>
    /// </summary>
    /// <typeparam name="E">The event delegate type</typeparam>
    public class WeakEvent<E>
    {
        private readonly object _eventHandlersLock = new object();
        private E _eventHandlers;
        private Func<IWeakEventRegistrationSignatures, E> _weakHandlerGetter;

        public WeakEvent(Func<IWeakEventRegistrationSignatures, E> weakHandlerGetter)
            : this(weakHandlerGetter, true)
        {
        }

        public WeakEvent(Func<IWeakEventRegistrationSignatures, E> weakHandlerGetter, bool registerForGCNotification)
        {
            _weakHandlerGetter = weakHandlerGetter;

            if (registerForGCNotification)
            {
                GCNotifier.GarbageCollected += OnGarbageCollected;
            }
        }

        internal void OnGarbageCollected()
        {
            foreach (var weakEvent in GetInvocationList().Select(handler => handler.Target).OfType<WeakEventRegistration>())
            {
                weakEvent.TryUnregister();
            }
        }

        public static WeakEvent<E> operator +(WeakEvent<E> weakEventHandlers, E newHandler)
        {
            weakEventHandlers.Add(newHandler);
            return weakEventHandlers;
        }

        public static WeakEvent<E> operator -(WeakEvent<E> weakEventHandlers, E handlerToRemove)
        {
            weakEventHandlers.Remove(handlerToRemove);
            return weakEventHandlers;
        }

        /// <summary>
        /// Returns the invocation list of the delegate.
        /// </summary>
        /// <returns>An array of delegates representing the invocation list of the current delegate.</returns>
        public Delegate[] GetInvocationList()
        {
            var eventHandlers = _eventHandlers;
            if (eventHandlers == null)
            {
                return new Delegate[0];
            }
            return ((Delegate)(eventHandlers as object)).GetInvocationList();
        }

        /// <summary>
        /// Raise at will.
        /// This also avoids the annoying .NET event invocation issue where when there are no registered handlers, the delegate needs to be checked
        /// for null. This version will just return an empty method body, when there are no registered handlers.
        /// </summary>
        public E Raise
        {
            get
            {
                var eventHandlers = _eventHandlers;
                if (eventHandlers == null)
                {
                    return _weakHandlerGetter(EmptyWeakEventRegistrationHandlers.Instance);
                }
                return eventHandlers;
            }
        }

        /// <summary>
        /// Add an event handler to the invocation list of event handlers. If the event handler is for an instance method will create
        /// a weak reference to the target instance, so its lifetime is not dependent on the lifetime of the event source
        /// </summary>
        /// <param name="handler">The handler to add</param>
        public void Add(E handler)
        {
            WeakEventRegistration.Register(
                eh =>
                {
                    lock (_eventHandlersLock)
                    {
                        _eventHandlers = (E)(Delegate.Combine((Delegate)(_eventHandlers as object), (Delegate)(eh as object)) as object);
                    }
                },
                handler,
                _weakHandlerGetter,
                eh =>
                {
                    lock (_eventHandlersLock)
                    {
                        _eventHandlers = (E)(Delegate.Remove((Delegate)(_eventHandlers as object), (Delegate)(eh as object)) as object);
                    }
                }
            );
        }

        /// <summary>
        /// Remove an event handler from the event handler invocation list.
        /// </summary>
        /// <param name="handler">The hander to remove</param>
        public void Remove(E handler)
        {
            WeakEventRegistration.Unregister(
                eh =>
                {
                    lock (_eventHandlersLock)
                    {
                        _eventHandlers = (E)(Delegate.Remove((Delegate)(_eventHandlers as object), (Delegate)(eh as object)) as object);
                        return _eventHandlers;
                    }
                },
                handler);
        }

    }
}