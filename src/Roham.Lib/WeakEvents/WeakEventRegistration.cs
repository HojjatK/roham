using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Roham.Lib.WeakEvents
{
    public interface IWeakEventRegistrationSignatures
    {
        void h();
        void h<T1>(T1 p1);
        void h<T1, T2>(T1 p1, T2 p2);
        void h<T1, T2, T3>(T1 p1, T2 p2, T3 p3);
        void h<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4);
        void h<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
        void h<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
        void h<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
    }

    internal class EmptyWeakEventRegistrationHandlers : IWeakEventRegistrationSignatures
    {
        private static EmptyWeakEventRegistrationHandlers sInstance = new EmptyWeakEventRegistrationHandlers();

        public static IWeakEventRegistrationSignatures Instance
        {
            get { return sInstance; }
        }

        public void h()
        {
        }

        public void h<P1>(P1 p1)
        {
        }

        public void h<P1, P2>(P1 p1, P2 p2)
        {
        }

        public void h<P1, P2, P3>(P1 p1, P2 p2, P3 p3)
        {
        }

        public void h<P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4)
        {
        }

        public void h<P1, P2, P3, P4, P5>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
        }

        public void h<P1, P2, P3, P4, P5, P6>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
        {
        }

        public void h<P1, P2, P3, P4, P5, P6, P7>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
        {
        }
    }

    internal abstract class WeakEventRegistration
    {
        private static Dictionary<DelegateTypeInfo, Func<Delegate, Action<IWeakEventRegistrationSignatures>, IWeakEventRegistrationSignatures>> CTorDict =
            new Dictionary<DelegateTypeInfo, Func<Delegate, Action<IWeakEventRegistrationSignatures>, IWeakEventRegistrationSignatures>>();
        private static ReaderWriterLockSlim CTorDictLock = new ReaderWriterLockSlim();

        private WeakReference _target;
        private MethodInfo _targetMethodInfo;

        public static void Register<E>(Action<E> registerAction, E handler, Func<IWeakEventRegistrationSignatures, E> weakHandlerCTor, Action<E> unregisterAction)
        {
            var handlerDelegate = (Delegate)(handler as object);
            if (handlerDelegate.Target == null)
            {
                registerAction(handler);
            }
            else
            {
                var weakHandler = Construct(handler, weh => unregisterAction(weakHandlerCTor(weh)));
                registerAction(weakHandlerCTor(weakHandler));
            }
        }

        public static void Unregister<E>(Func<E, E> unregisterFunc, E handler)
        {
            if (unregisterFunc == null)
            {
                return;
            }

            var @event = unregisterFunc(handler);
            var eventDelegate = (Delegate)(@event as object);
            var handlerDelegate = (Delegate)(handler as object);
            if (eventDelegate == null)
            {
                return;
            }
            foreach (var @delegate in eventDelegate.GetInvocationList())
            {
                var weakHandler = @delegate.Target as WeakEventRegistration;
                if (weakHandler != null)
                {
                    if (weakHandler.Target == handlerDelegate.Target && weakHandler.TargetMethodInfo == handlerDelegate.Method)
                    {
                        unregisterFunc((E)(@delegate as object));
                        break;
                    }
                }
            }
        }

        public object Target => _target.Target;

        public MethodInfo TargetMethodInfo => _targetMethodInfo;

        public bool TryUnregister()
        {
            if (Target == null)
            {
                Unregister();
                return true;
            }
            return false;
        }

        protected WeakEventRegistration(object target, MethodInfo targetMethodInfo)
        {
            _target = new WeakReference(target);
            _targetMethodInfo = targetMethodInfo;
        }

        protected void AddCTor<E>(Delegate @delegate, Func<Delegate, Action<IWeakEventRegistrationSignatures>, IWeakEventRegistrationSignatures> cTor)
        {
            CTorDict[new DelegateTypeInfo(@delegate.Method.DeclaringType, typeof(E))] = cTor;
        }

        protected static IWeakEventRegistrationSignatures Construct<E>(E @delegate, Action<IWeakEventRegistrationSignatures> unregister)
        {
            Func<Delegate, Action<IWeakEventRegistrationSignatures>, IWeakEventRegistrationSignatures> cTor = null;
            var delegateObj = @delegate as Delegate;
            var lTypeInfo = new DelegateTypeInfo(delegateObj.Method.DeclaringType, typeof(E));
            using (CTorDictLock.ReadScope())
            {
                if (!CTorDict.TryGetValue(lTypeInfo, out cTor))
                {
                    cTor = null;
                }
            }
            if (cTor != null)
            {
                return cTor(delegateObj, unregister);
            }
            using (CTorDictLock.WriteScope())
            {
                if (!CTorDict.TryGetValue(lTypeInfo, out cTor))
                {
                    var genericType = typeof(WeakEventRegistration<,>).CreateGenericType(delegateObj.Method.DeclaringType, typeof(E));
                    var weakHandler = genericType.CreateInstance(delegateObj, unregister, true).CastTo<IWeakEventRegistrationSignatures>();
                    return weakHandler;
                }
            }
            return cTor(delegateObj, unregister);

        }

        protected abstract void Unregister();

        #region Nested Types

        private struct DelegateTypeInfo
        {
            private Type _targetType;
            private Type _delegateType;

            public DelegateTypeInfo(Type targetType, Type delegateType)
            {
                _targetType = targetType;
                _delegateType = delegateType;
            }

            public override bool Equals(object obj)
            {
                var rhsInfo = (DelegateTypeInfo)obj;
                return (_targetType == rhsInfo._targetType && _delegateType == rhsInfo._delegateType);
            }

            public override int GetHashCode()
            {
                return (_targetType.GetHashCode() ^ _delegateType.GetHashCode());
            }
        }

        #endregion
    }

    internal class WeakEventRegistration<T, E> : WeakEventRegistration, IWeakEventRegistrationSignatures where T : class
    {
        private static readonly Type[] sActionTypes = new[] { typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>), typeof(Action<,,,,>), typeof(Action<,,,,,>), typeof(Action<,,,,,,>), typeof(Action<,,,,,,,>) };
        private static Dictionary<MethodInfo, Delegate> MethodToDelegateMap = new Dictionary<MethodInfo, Delegate>();
        private static ReaderWriterLockSlim MethodToDelegateMapLock = new ReaderWriterLockSlim();
        private static Type OpenHandlerType = null;

        private Delegate _openHandler = null;
        private Action<IWeakEventRegistrationSignatures> _unregister;

        public WeakEventRegistration(Delegate @delegate, Action<IWeakEventRegistrationSignatures> unregister, bool addToDict)
            : base(@delegate.Target, @delegate.Method)
        {
            if (addToDict)
            {
                base.AddCTor<E>(@delegate, (del, unreg) => new WeakEventRegistration<T, E>(del, unreg));
            }
            _unregister = unregister;
            using (MethodToDelegateMapLock.ReadScope())
            {
                if (MethodToDelegateMap.TryGetValue(@delegate.Method, out _openHandler))
                {
                    return;
                }
            }
            using (MethodToDelegateMapLock.WriteScope())
            {
                if (MethodToDelegateMap.TryGetValue(@delegate.Method, out _openHandler))
                {
                    return;
                }
                if (OpenHandlerType == null)
                {
                    var parmInfo = @delegate.Method.GetParameters();
                    var types = new Type[parmInfo.Length + 1];
                    var index = 0;
                    types[index++] = @delegate.Method.DeclaringType;
                    foreach (var info in parmInfo)
                    {
                        types[index] = info.ParameterType;
                        index++;
                    }
                    if (types.Length > 8)
                    {
                        throw new NotSupportedException("Event Handlers with number of parameters greater than 7 are not supported");
                    }
                    OpenHandlerType = sActionTypes[types.Length - 1].CreateGenericType(types);
                }
                _openHandler = Delegate.CreateDelegate(OpenHandlerType, null, @delegate.Method);
                MethodToDelegateMap.Add(@delegate.Method, _openHandler);
            }

        }

        public WeakEventRegistration(Delegate @delegate, Action<IWeakEventRegistrationSignatures> unregister)
            : this(@delegate, unregister, false)
        {
        }

        public void h()
        {
            InvokeHandler(target => ((Action<T>)(_openHandler as object))(target));
        }

        public void h<T1>(T1 p1)
        {
            InvokeHandler(target => ((Action<T, T1>)(_openHandler as object))(target, p1));
        }

        public void h<T1, T2>(T1 p1, T2 p2)
        {
            InvokeHandler(target => ((Action<T, T1, T2>)(_openHandler as object))(target, p1, p2));
        }

        public void h<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
        {
            InvokeHandler(target => ((Action<T, T1, T2, T3>)(_openHandler as object))(target, p1, p2, p3));
        }

        public void h<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            InvokeHandler(target => ((Action<T, T1, T2, T3, T4>)(_openHandler as object))(target, p1, p2, p3, p4));
        }

        public void h<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            InvokeHandler(target => ((Action<T, T1, T2, T3, T4, T5>)(_openHandler as object))(target, p1, p2, p3, p4, p5));
        }

        public void h<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            InvokeHandler(target => ((Action<T, T1, T2, T3, T4, T5, T6>)(_openHandler as object))(target, p1, p2, p3, p4, p5, p6));
        }

        public void h<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            InvokeHandler(target => ((Action<T, T1, T2, T3, T4, T5, T6, T7>)(_openHandler as object))(target, p1, p2, p3, p4, p5, p6, p7));
        }

        protected override void Unregister()
        {
            if (_unregister != null)
                _unregister(this);
        }

        protected void InvokeHandler(Action<T> invocation)
        {
            T target = Target as T;
            if (target != null)
            {
                invocation(target);
            }
            else
            {
                Unregister();
            }
        }
    }
}
