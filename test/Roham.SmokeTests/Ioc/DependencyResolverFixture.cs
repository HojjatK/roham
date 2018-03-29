using System;
using Autofac;
using AF = Autofac;
using Roham.Lib.Ioc;
using Roham.Ioc.Autofac;
using IOC = Roham.Lib.Ioc;
using System.Collections.Generic;

namespace Roham.SmokeTests.Ioc
{
    public abstract class DependencyResolverFixture
    {
        public interface IFoo
        {
            string Name { get; }
            decimal FooTest(decimal value1, int value2);
        }

        public interface IFoo<T> : IFoo
        {
            T TObject { get; }
        }

        public interface IBar
        {
            string Name { get; }
        }

        public interface IBaz
        {
            IFoo Foo { get; }
            IBar Bar { get; }
            string Name { get; }
        }
        

        public interface IQuery<out TResult>
        {
        }

        public class FooQuery<TDto, TEntity> : IQuery<List<TDto>>
        {   
        }

        public class FooQuery2<TDto, TEntity> : IQuery<List<TDto>>
        {
        }

        public class BarQuery<TDto, TEntity> : IQuery<TDto>
        {
        }

        public class FooQueryClosed : IQuery<List<string>>
        {
        }

        public interface IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {   
        }

        public abstract class QueryHandlerAbstract<TQuery, TResult> : IQueryHandler<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
        }

        [AutoRegister]
        public class FooQueryHandler<TDto, TEntity> : QueryHandlerAbstract<FooQuery<TDto, TEntity>, List<TDto>>
        {
        }

        [AutoRegister]
        public class FooQueryHandler2<TDto, TEntity> : QueryHandlerAbstract<FooQuery2<TDto, TEntity>, List<TDto>>
        {
        }

        [AutoRegister]
        public class FooQueryClosedQueryHandler : IQueryHandler<FooQueryClosed, List<string>>
        {
        }

        [AutoRegister]
        public class BarQueryHandler<TDto, TEntity> : QueryHandlerAbstract<BarQuery<TDto, TEntity>, TDto>
        {
        }

        [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
        public class Foo : IFoo
        {
            public virtual string Name
            {
                get { return "Foo"; }
            }

            public decimal FooTest(decimal value1, int value2)
            {
                return value1 * value2 * (decimal)0.5;
            }
        }

        [AutoRegister]
        public class FooImpl : IFoo
        {
            public virtual string Name
            {
                get { return "FooImpl"; }
            }

            public decimal FooTest(decimal value1, int value2)
            {
                return value1 * value2;
            }
        }

        [AutoRegister]
        public class Foo<T> : FooImpl, IFoo<T>
        {
            protected T _obj;
            public T TObject { get { return _obj; } }
        }

        [AutoRegister]
        public class FooImpl2 : Foo<Guid>
        {
            public FooImpl2()
            {
                _obj = Guid.NewGuid();
            }

            public override string Name
            {
                get { return "FooImpl2"; }
            }
        }

        [AutoRegister]
        public class BazImpl : IBaz
        {
            public BazImpl(IFoo foo, IBar bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public string Name
            {
                get { return "BazImpl"; }
            }

            public IFoo Foo { get; private set; }
            public IBar Bar { get; private set; }
        }

        [AutoRegister(LifetimeScope = LifetimeScopeType.InstancePerDependency)]
        public class Bar : IBar
        {
            public string Name
            {
                get { return "Bar"; }
            }
        }

        [AutoRegister]
        public class BarImpl : IBar
        {
            public string Name
            {
                get { return "BarImpl"; }
            }
        }

        public class RohamTestDependencyResolver : IOC.AbstractDependencyResolver
        {
            public RohamTestDependencyResolver(IOC.ILifetimeScope rootScope) : base(rootScope) { }
        }

        protected RohamTestDependencyResolver CreateSubject(Action<AF.ContainerBuilder, IOC.IAutoRegistration> registrationAction)
        {
            if (registrationAction == null)
            {
                throw new NotImplementedException("registrationAction");
            }
            var builder = new AF.ContainerBuilder();
            var autoRegistration = new IOC.AutoRegistration(AutofacIocFactory.CreateRegistrator(builder));
            registrationAction(builder, autoRegistration);

            builder
                .RegisterType(AutofacIocFactory.GetLifetimeScopeType())
                .As<IOC.IResolver>()
                .As<IOC.ILifetimeScope>()
                .InstancePerLifetimeScope();

            var container = builder.Build();
            var rootScope = container.Resolve<IOC.ILifetimeScope>();
            return new RohamTestDependencyResolver(rootScope);
        }

    }
}
