using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using System.Reflection;

namespace Roham.Lib.Proxy
{
    public interface IDynamicProxyBuilder
    {
        DynamicProxyBuilder.IProxyForClassBuilderContext<TClass> ForClass<TClass>() where TClass : class;
        DynamicProxyBuilder.IProxyForInterfaceBuilderContext<TInterface> ForInterface<TInterface>() where TInterface : class;
    }

    public class DynamicProxyBuilder : IDynamicProxyBuilder
    {
        private static ProxyGenerator ProxyGenerator = new ProxyGenerator();

        private enum ProxyTypes
        {
            Class,
            Interface
        }

        public IProxyForClassBuilderContext<TClass> ForClass<TClass>() where TClass : class
        {
            return new DynamicProxyBuilderState<TClass>(ProxyTypes.Class);
        }

        public IProxyForInterfaceBuilderContext<TInterface> ForInterface<TInterface>() where TInterface : class
        {
            return new DynamicProxyBuilderState<TInterface>(ProxyTypes.Interface);
        }

        #region Nested Interfaces/Classes

        public interface IProxyForClassBuilderContext<TClass> where TClass : class
        {
            IProxyBuilderIntercetorsContext<TClass> WithTarget(TClass target);
            IProxyBuilderIntercetorsContext<TClass> WithoutTraget();
        }

        public interface IProxyForInterfaceBuilderContext<TInterface> where TInterface : class
        {
            IProxyBuilderIntercetorsContext<TInterface> WithTarget(TInterface target);
            IProxyBuilderIntercetorsContext<TInterface> WithTargetInterface(TInterface targetInterface);
            IProxyBuilderIntercetorsContext<TInterface> WithoutTraget();
        }

        public interface IProxyBuilderIntercetorsContext<T> where T : class
        {
            IProxyBuilderOptionsContext<T> AddInterceptor(IInterceptor interceptor);
            IProxyBuilderOptionsContext<T> AddInterceptors(IInterceptor interceptor, params IInterceptor[] extraInterceptors);
        }

        public interface IProxyBuilderOptionsContext<T> where T : class
        {
            IProxyBuilderOptionsContext<T> WithMixin(object mixinInstance);
            IProxyBuilderOptionsContext<T> ShouldNotIntercept(Type type, MethodInfo methodInfo);
            T Build();
        }

        private class DynamicProxyBuilderState<T> :
            IProxyForClassBuilderContext<T>,
            IProxyForInterfaceBuilderContext<T>,
            IProxyBuilderIntercetorsContext<T>,
            IProxyBuilderOptionsContext<T>
            where T : class
        {
            private readonly ProxyTypes _proxyType;
            private T _target = null;
            private T _targetInterface = null;
            private readonly List<IInterceptor> _interceptors = new List<IInterceptor>();
            private readonly List<object> _mixins = new List<object>();
            private readonly List<Tuple<Type, MethodInfo>> _filterMethods = new List<Tuple<Type, MethodInfo>>();

            public DynamicProxyBuilderState(ProxyTypes proxyType)
            {
                _proxyType = proxyType;
            }

            IProxyBuilderIntercetorsContext<T> IProxyForClassBuilderContext<T>.WithTarget(T target)
            {
                _target = target;
                return this;
            }

            IProxyBuilderIntercetorsContext<T> IProxyForClassBuilderContext<T>.WithoutTraget()
            {
                _target = null;
                return this;
            }

            IProxyBuilderIntercetorsContext<T> IProxyForInterfaceBuilderContext<T>.WithTarget(T target)
            {
                _target = target;
                _targetInterface = null;
                return this;
            }

            IProxyBuilderIntercetorsContext<T> IProxyForInterfaceBuilderContext<T>.WithTargetInterface(T targetInterface)
            {
                _target = null;
                _targetInterface = targetInterface;
                return this;
            }

            IProxyBuilderIntercetorsContext<T> IProxyForInterfaceBuilderContext<T>.WithoutTraget()
            {
                _target = null;
                _targetInterface = null;
                return this;
            }


            IProxyBuilderOptionsContext<T> IProxyBuilderIntercetorsContext<T>.AddInterceptor(IInterceptor interceptor)
            {
                AddInterceptorInternal(interceptor);
                return this;
            }

            IProxyBuilderOptionsContext<T> IProxyBuilderIntercetorsContext<T>.AddInterceptors(IInterceptor interceptor, params IInterceptor[] extraInterceptors)
            {
                AddInterceptorInternal(interceptor);

                if (extraInterceptors != null)
                {
                    extraInterceptors
                        .ForEach(i => AddInterceptorInternal(i));
                }

                return this;
            }

            IProxyBuilderOptionsContext<T> IProxyBuilderOptionsContext<T>.WithMixin(object mixinInstance)
            {
                _mixins.Add(mixinInstance);

                return this;
            }

            IProxyBuilderOptionsContext<T> IProxyBuilderOptionsContext<T>.ShouldNotIntercept(Type type, MethodInfo methodInfo)
            {
                _filterMethods.Add(new Tuple<Type, MethodInfo>(type, methodInfo));
                return this;
            }

            T IProxyBuilderOptionsContext<T>.Build()
            {
                T result = null;
                var options = CreateOptions();

                if (_proxyType == ProxyTypes.Interface)
                {
                    if (options != null)
                    {
                        result = _target != null ?
                            ProxyGenerator.CreateInterfaceProxyWithTarget<T>(_target, options, _interceptors.ToArray()) :
                            _targetInterface != null ?
                            ProxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(_target, options, _interceptors.ToArray()) :
                            ProxyGenerator.CreateInterfaceProxyWithoutTarget<T>(options, _interceptors.ToArray());
                    }
                    else
                    {
                        result = _target != null ?
                            ProxyGenerator.CreateInterfaceProxyWithTarget<T>(_target, _interceptors.ToArray()) :
                            _targetInterface != null ?
                            ProxyGenerator.CreateInterfaceProxyWithTargetInterface<T>(_target, _interceptors.ToArray()) :
                            ProxyGenerator.CreateInterfaceProxyWithoutTarget<T>(_interceptors.ToArray());
                    }
                }
                if (_proxyType == ProxyTypes.Class)
                {
                    if (options != null)
                    {
                        result = _target != null ?
                            ProxyGenerator.CreateClassProxyWithTarget<T>(_target, options, _interceptors.ToArray()) :
                            ProxyGenerator.CreateClassProxy<T>(options, _interceptors.ToArray());
                    }
                    else
                    {
                        result = _target != null ?
                            ProxyGenerator.CreateClassProxyWithTarget<T>(_target, _interceptors.ToArray()) :
                            ProxyGenerator.CreateClassProxy<T>(_interceptors.ToArray());
                    }
                }
                return result;
            }

            private void AddInterceptorInternal(IInterceptor interceptor)
            {
                _interceptors.Add(interceptor);
            }

            private ProxyGenerationOptions CreateOptions()
            {
                if (_filterMethods.Any() || _mixins.Any())
                {
                    var options = new ProxyGenerationOptions();
                    if (_filterMethods.Any())
                    {
                        options.Hook = new ProxyGenerationHookImpl(_filterMethods);
                    }
                    _mixins.ForEach(mi => options.AddMixinInstance(mi));

                    return options;
                }

                return null;
            }

            private class ProxyGenerationHookImpl : IProxyGenerationHook
            {
                private readonly List<Tuple<Type, MethodInfo>> _filterMethods;

                public ProxyGenerationHookImpl(
                    List<Tuple<Type, MethodInfo>> filterMethods)
                {
                    _filterMethods = filterMethods;
                }

                public void MethodsInspected() { }

                public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) { }

                public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
                {
                    bool any =
                        _filterMethods.Any(mi => mi.Item1.Equals(type) && mi.Item2.Equals(methodInfo));
                    return !any;
                }
            }
        }

        #endregion
    }
}