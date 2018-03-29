using System.Collections.Generic;
using NUnit.Framework;
using Autofac;

namespace Roham.SmokeTests.Ioc
{
    public class DependencyResolverTest
    {
        [TestFixture]
        [Category("SmokeTests.Ioc.DependencyResolver")]
        internal class GivenRohamDependencyResolver : DependencyResolverFixture
        {
            [Test]
            public void TestRegisterInstance()
            {
                var fooInstance1 = new FooImpl();
                var fooInstance2 = new FooImpl2();

                // given
                using (var dependencyResolver = CreateSubject((builder, _) => {
                    builder
                        .RegisterInstance<FooImpl>(fooInstance1)
                        .As<IFoo>();
                }))
                {
                    // when
                    var foo1 = dependencyResolver.Resolve<IFoo>();
                    var foo2 = dependencyResolver.Resolve<IFoo>();
                    IFoo foo3, foo4;
                    using (var scope1 = dependencyResolver.BeginLifetimeScope())
                    {
                        foo3 = scope1.Resolve<IFoo>();
                        using (var scope2 = scope1.BeginLifetimeScope(r => r.RegisterInstance<IFoo>(fooInstance2)))
                        {
                            foo4 = scope2.Resolve<IFoo>();
                        }
                    }

                    // assert
                    new List<object>() { foo1, foo2, foo3 }.ForEach(foo => {
                        Assert.IsNotNull(foo);
                        Assert.AreEqual(typeof(FooImpl).FullName, foo.GetType().FullName);
                    });
                    Assert.IsNotNull(foo4);
                    Assert.AreEqual(typeof(FooImpl2).FullName, foo4.GetType().FullName);

                    Assert.IsTrue(foo1 == fooInstance1 && foo2 == fooInstance1 && foo3 == fooInstance1);
                    Assert.IsTrue(foo4 == fooInstance2);
                }
            }

            [Test]
            public void TestRegisterPerDependency()
            {
                // given
                using (var dependencyResolver = CreateSubject((builder, _) => {
                    builder.RegisterType<BarImpl>()
                        .As<IBar>();
                    builder.RegisterType<FooImpl>()
                        .As<IFoo>();
                }))
                {
                    // when
                    var foo1 = dependencyResolver.Resolve<IFoo>();
                    var foo2 = dependencyResolver.Resolve<IFoo>();
                    object foo3 = null, foo4 = null;
                    using (var scope1 = dependencyResolver.BeginLifetimeScope())
                    {
                        foo3 = scope1.Resolve<IFoo>();
                        using (var scope2 = scope1.BeginLifetimeScope())
                        {
                            foo4 = scope2.Resolve<IFoo>();
                        }
                    }

                    // assert
                    new List<object>() { foo1, foo2, foo3, foo4 }.ForEach(foo => {
                        Assert.IsNotNull(foo);
                        Assert.AreEqual(typeof(FooImpl).FullName, foo.GetType().FullName);
                    });

                    Assert.IsTrue(foo1 != foo2 && foo1 != foo3 && foo1 != foo4);
                    Assert.IsTrue(foo2 != foo3 && foo2 != foo4);
                    Assert.IsTrue(foo3 != foo4);
                }
            }

            [Test]
            public void TestRegisterPerLifetimeScope()
            {
                // given
                using (var dependencyResolver = CreateSubject((builder, _) => {
                    builder.RegisterType<BarImpl>()
                        .As<IBar>();

                    builder.RegisterType<FooImpl>()
                        .As<IFoo>()
                        .InstancePerLifetimeScope();
                }))
                {
                    // when
                    var foo1 = dependencyResolver.Resolve<IFoo>();
                    object foo2 = null, foo3 = null, foo4 = null, foo5 = null;
                    using (var scope1 = dependencyResolver.BeginLifetimeScope())
                    {
                        foo2 = scope1.Resolve<IFoo>();
                        foo3 = scope1.Resolve<IFoo>();
                        using (var scope2 = scope1.BeginLifetimeScope())
                        {
                            foo4 = scope2.Resolve<IFoo>();
                            foo5 = scope2.Resolve<IFoo>();
                        }
                    }

                    // assert
                    new List<object>() { foo1, foo2, foo3, foo4, foo5 }.ForEach(foo => {
                        Assert.IsNotNull(foo);
                        Assert.AreEqual(typeof(FooImpl).FullName, foo.GetType().FullName);
                    });

                    Assert.IsTrue(foo1 != foo2 && foo1 != foo3 && foo1 != foo4 && foo1 != foo5);
                    Assert.IsTrue(foo2 == foo3 && foo2 != foo4 && foo2 != foo5);
                    Assert.IsTrue(foo3 != foo4 && foo3 != foo4);
                    Assert.IsTrue(foo4 == foo5);
                }
            }

            [Test]
            public void TestRegisterPerRequest()
            {
                // given
                using (var dependencyResolver = CreateSubject((builder, _) => {
                    builder.RegisterType<FooImpl>()
                        .As<IFoo>()
                        .InstancePerDependency();

                    builder.RegisterType<BarImpl>()
                        .As<IBar>()
                        .InstancePerDependency();

                    builder.RegisterType<BazImpl>()
                        .As<IBaz>()
                        .InstancePerRequest();
                }))
                {
                    // when
                    IFoo foo1 = dependencyResolver.Resolve<IFoo>();
                    IBar bar1 = dependencyResolver.Resolve<IBar>();

                    IBaz baz1, baz2, baz3;
                    using (var scope = dependencyResolver.BeginLifetimeScope("AutofacWebRequest"))
                    {
                        baz1 = scope.Resolve<IBaz>();
                        baz2 = scope.Resolve<IBaz>();
                        using (var innerScope = scope.BeginLifetimeScope())
                        {
                            baz3 = innerScope.Resolve<IBaz>();
                        }
                    }

                    // assert
                    new List<object>() { baz1, baz2, baz3 }.ForEach(baz => {
                        Assert.IsNotNull(baz);
                        Assert.AreEqual(typeof(BazImpl).FullName, baz.GetType().FullName);
                    });

                    Assert.IsTrue(baz1 == baz2 && baz1 == baz3);
                    Assert.IsTrue(baz2 == baz3);

                    Assert.IsTrue(foo1 != baz1.Foo && bar1 != baz1.Bar);
                    Assert.IsTrue(baz1.Foo == baz2.Foo);
                }
            }

            [Test]
            public void TestRegisterSingleInstance()
            {
                // given
                using (var dependencyResolver = CreateSubject((builder, _) => {
                    builder.RegisterType<BarImpl>()
                        .As<IBar>();

                    builder.RegisterType<FooImpl>()
                        .As<IFoo>()
                        .SingleInstance();
                }))
                {
                    // when
                    var foo1 = dependencyResolver.Resolve<IFoo>();
                    var foo2 = dependencyResolver.Resolve<IFoo>();
                    object foo3 = null, foo4 = null;
                    using (var scope1 = dependencyResolver.BeginLifetimeScope())
                    {
                        foo3 = scope1.Resolve<IFoo>();
                        using (var scope2 = scope1.BeginLifetimeScope())
                        {
                            foo4 = scope2.Resolve<IFoo>();
                        }
                    }

                    // assert
                    new List<object>() { foo1, foo2, foo3, foo4 }.ForEach(foo => {
                        Assert.IsNotNull(foo);
                        Assert.AreEqual(typeof(FooImpl).FullName, foo.GetType().FullName);
                    });

                    Assert.IsTrue(foo1 == foo2);
                    Assert.IsTrue(foo2 == foo3);
                    Assert.IsTrue(foo3 == foo4);
                }
            }
        }
    }
}
