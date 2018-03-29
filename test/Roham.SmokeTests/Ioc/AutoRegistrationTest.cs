using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Roham.Lib.Ioc;

namespace Roham.SmokeTests.Ioc
{
    public class AutoRegistrationTest
    {
        [TestFixture]
        [Category("SmokeTests.Ioc.AutoRegistration")]
        internal class GivenAutoRegistration : DependencyResolverFixture
        {
            [Test]
            public void TestIncludedAssemblyFromType()
            {
                // given
                IAutoRegistration autoRegistration = null;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when
                        ar
                        .IncludeAssembilesFromType(typeof(IFoo))
                        .ApplyRegistrations();
                        autoRegistration = ar;
                    }))
                {

                    // then
                    Assert.AreEqual(1, autoRegistration.IncludeAssemblies.Count(), "there should be only one assembly included");
                    Assert.AreEqual("Roham.SmokeTests.dll", autoRegistration.IncludeAssemblies.Single().ManifestModule.Name, "assembly name is invalid");
                }
            }

            [Test]
            public void TestIncludeAssembilesFromTypes()
            {
                // given
                IAutoRegistration autoRegistration = null;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when 
                        ar
                        .IncludeAssembilesFromTypes(new List<Type> { typeof(IFoo), typeof(IBar) })
                        .ApplyRegistrations();
                        autoRegistration = ar;
                    }))
                {

                    // then
                    Assert.AreEqual(1, autoRegistration.IncludeAssemblies.Count(), "there should be only one assembly included");
                    Assert.AreEqual("Roham.SmokeTests.dll", autoRegistration.IncludeAssemblies.Single().ManifestModule.Name, "assembly name is invalid");
                }
            }

            [Test]
            public void TestIncludeAssembliesFromCurrentDomain()
            {
                // given
                IAutoRegistration autoRegistration = null;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when 
                        ar
                        .IncludeAssembliesFromCurrentDomain(a => a.FullName.StartsWith("Roham.SmokeTests"))
                        .ApplyRegistrations();
                        autoRegistration = ar;
                    }))
                {
                    // then
                    Assert.AreEqual(1, autoRegistration.IncludeAssemblies.Count(), "there should be only one assembly included");
                    Assert.AreEqual("Roham.SmokeTests.dll", autoRegistration.IncludeAssemblies.Single().ManifestModule.Name, "assembly name is invalid");
                }
            }

            [Test]
            public void TestIncludeAssembliesFromFiles()
            {
                // given
                var filePath = Assembly.GetExecutingAssembly().Location;
                IAutoRegistration autoRegistration = null;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when 
                        ar
                        .IncludeAssembliesFromFiles(new string[] { filePath })
                        .ApplyRegistrations();
                        autoRegistration = ar;
                    }))
                {
                    // then
                    Assert.AreEqual(1, autoRegistration.IncludeAssemblies.Count(), "there should be only one assembly included");
                    Assert.AreEqual("Roham.SmokeTests.dll", autoRegistration.IncludeAssemblies.Single().ManifestModule.Name, "assembly name is invalid");
                }
            }

            [Test]
            public void TestExcludeSystemAssemblies()
            {
                // given                
                IEnumerable<Assembly> sysAssembliesIncluded = null;
                IEnumerable<Assembly> sysAssembliesExcluded = null;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when 
                        ar
                        .IncludeAssembliesFromCurrentDomain();
                        sysAssembliesIncluded = ar.IncludeAssemblies;

                        ar
                        .ExcludeSystemAssemblies()
                        .ApplyRegistrations();
                        sysAssembliesExcluded = ar.IncludeAssemblies;
                    }))
                {
                    // then
                    Assert.IsTrue(sysAssembliesIncluded.Any(a => a.FullName.StartsWith("System")));
                    Assert.IsFalse(sysAssembliesExcluded.Any(a => a.FullName.StartsWith("System")));
                }
            }

            [Test]
            public void TestExcludeAssemblies()
            {
                IEnumerable<Assembly> sysAssembliesIncluded = null;
                IEnumerable<Assembly> sysAssembliesExcluded = null;

                var filePath = Assembly.GetExecutingAssembly().Location;
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when 
                        ar
                        .IncludeAssembliesFromCurrentDomain();
                        sysAssembliesIncluded = ar.IncludeAssemblies;

                        ar
                        .ExcludeAssemblies(a => a.FullName.StartsWith("System"))
                        .ApplyRegistrations();
                        sysAssembliesExcluded = ar.IncludeAssemblies;
                    }))
                {
                    // then
                    Assert.IsTrue(sysAssembliesIncluded.Any(a => a.FullName.StartsWith("System")));
                    Assert.IsFalse(sysAssembliesExcluded.Any(a => a.FullName.StartsWith("System")));
                }
            }

            [Test]
            public void TestIncludeImplementsITypeNameConvention()
            {
                // given                
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when
                        ar
                        .IncludeAssembilesFromType(typeof(IFoo))
                        .IncludeImplementsITypeNameConvention()
                        .ApplyRegistrations();
                    }))
                {
                    // then
                    var foo1 = dependencyResolver.Resolve<IFoo>();
                    var foo2 = dependencyResolver.Resolve<IFoo>();
                    var bar1 = dependencyResolver.Resolve<IBar>();
                    var bar2 = dependencyResolver.Resolve<IBar>();
                    Assert.AreEqual(typeof(Foo), foo1.GetType(), "IFoo is not resolved as Foo");
                    Assert.AreEqual(typeof(Foo), foo2.GetType(), "IFoo is not resolved as Foo");

                    Assert.AreEqual(typeof(Bar), bar1.GetType(), "IBar is not resolved as Bar");
                    Assert.AreEqual(typeof(Bar), bar2.GetType(), "IBar is not resolved as Bar");

                    // Foo auto registerd as single instance, and bar as per dependency
                    Assert.IsTrue(foo1 == foo2);
                    Assert.IsTrue(bar1 != bar2);
                }
            }

            [Test]
            public void TestIncludeClosingTypeConvention()
            {
                // given
                using (var dependencyResolver = CreateSubject(
                    (_, ar) =>
                    {
                        // when
                        ar
                        .IncludeAssembilesFromType(typeof(IFoo))
                        .IncludeClosingTypeConvention()
                        .ApplyRegistrations();
                    }))
                {
                    // then
                    var foo1 = dependencyResolver.Resolve<IFoo<Guid>>();
                    var foo2 = dependencyResolver.Resolve<IFoo<int>>();

                    Assert.AreEqual(typeof(FooImpl2), foo1.GetType(), "IFoo is not resolved as Foo<Guid>");
                    Assert.AreEqual(typeof(Foo<int>), foo2.GetType(), "IFoo is not resolved as Foo<>");

                    var fooHandler = dependencyResolver.Resolve<IQueryHandler<FooQuery<int, Foo>, List<int>>>();
                    var fooHandler2 = dependencyResolver.Resolve<IQueryHandler<FooQuery2<int, Foo>, List<int>>>();
                    var fooClosedHandler = dependencyResolver.Resolve<IQueryHandler<FooQueryClosed, List<string>>>();
                    var barHandler = dependencyResolver.Resolve<IQueryHandler<BarQuery<int, Bar>, int>>();

                    Assert.AreEqual(typeof(FooQueryHandler<int, Foo>), fooHandler.GetType(), "IQueryHandler<FooQuery<int, Foo>, List<int>> is not resolved as FooQueryHandler<int, Foo>");
                    Assert.AreEqual(typeof(FooQueryHandler2<int, Foo>), fooHandler2.GetType(), "IQueryHandler<FooQuery2<int, Foo>, List<int>> is not resolved as FooQueryHandler2<int, Foo>");
                    Assert.AreEqual(typeof(FooQueryClosedQueryHandler), fooClosedHandler.GetType(), "IQueryHandler<FooQueryClosed, List<string>> is not resolved as FooQueryClosedQueryHandler");
                    Assert.AreEqual(typeof(BarQueryHandler<int, Bar>), barHandler.GetType(), "IQueryHandler<BarQuery<int, Bar>, int> is not resolved as BarQueryHandler<int, Bar>");

                    AssertHandlerResolutionWithMakeGenericType(dependencyResolver, new FooQuery<Foo, Foo>(), typeof(FooQueryHandler<Foo, Foo>));
                    AssertHandlerResolutionWithMakeGenericType(dependencyResolver, new FooQuery2<Foo, Foo>(), typeof(FooQueryHandler2<Foo, Foo>));
                    AssertHandlerResolutionWithMakeGenericType(dependencyResolver, new FooQueryClosed(), typeof(FooQueryClosedQueryHandler));
                    AssertHandlerResolutionWithMakeGenericType(dependencyResolver, new BarQuery<Bar, Bar>(), typeof(BarQueryHandler<Bar, Bar>));
                }
            }

            private void AssertHandlerResolutionWithMakeGenericType<TResult>(
                RohamTestDependencyResolver dependencyResolver,
                IQuery<TResult> query,
                Type expectedType)
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
                dynamic resolvedHandlerType = dependencyResolver.Resolve(handlerType);
                Assert.AreEqual(expectedType, resolvedHandlerType.GetType(), $"{resolvedHandlerType.GetType()} is not resolved as {expectedType}");
            }
        }
    }
}