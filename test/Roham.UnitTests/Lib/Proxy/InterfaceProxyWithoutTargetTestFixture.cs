using System;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class InterfaceProxyWithoutTargetTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithoutTarget_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var testInterceptor1 = new TestInterceptor3(() => 1);
                var testInterceptor2 = new TestInterceptor4(() => 2);
                IClassC proxy = subjectBuilder
                    .ForInterface<IClassC>()
                    .WithoutTraget()
                    .AddInterceptors(testInterceptor1, testInterceptor2)
                    .Build();

                // When
                int aValue = 0;
                proxy.CMethod(() => aValue = 10); // the test method does have target and action is not called
                var retValue = proxy.CP1; // the first interceptor in the chain determine the return value

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(2, testInterceptor1.InterceptReceivedCall);
                Assert.AreEqual(2, testInterceptor2.InterceptReceivedCall);
                Assert.AreEqual(0, aValue);
                Assert.AreEqual(1, retValue);
            }

            // TODO: test mixin + filter
        }

        internal interface ITestClassB
        {
            void TestMethod(Action action);
            string TestProperty { get; set; }
            int TestProperty2 { get; set; }
        }

        internal class TestClassB : ITestClassB
        {
            public virtual void TestMethod(Action action)
            {
                action();
            }

            public virtual string TestProperty { get; set; }

            public int TestProperty2 { get; set; }
        }


        internal abstract class TestInterceptor : IInterceptor
        {
            public virtual void Intercept(IInvocation invk)
            {
                InterceptReceivedCall++;
                Console.WriteLine("{0} called", GetType().Name);
                Console.WriteLine(" InvocationTarget: {0}\t Target:{1}", invk.InvocationTarget.GetType().Name, invk.TargetType.Name);
                Console.WriteLine(" MethodInvocationTarget: {0}\t Method:{1}", invk.MethodInvocationTarget.Name, invk.Method.Name);

                invk.Proceed();

                Console.WriteLine("{0} ReturnValue: {1}", GetType().Name, invk.ReturnValue == null ? "Null" : invk.ReturnValue);
            }

            public int InterceptReceivedCall
            {
                get;
                protected set;
            }

        }

        internal class TestInterceptor3 : TestInterceptor
        {
            protected readonly Func<int> _f;
            public TestInterceptor3(Func<int> f)
            {
                _f = f;
            }

            public override void Intercept(IInvocation memberInvocation)
            {
                InterceptReceivedCall++;

                memberInvocation.Proceed();

                memberInvocation.ReturnValue = _f();
            }
        }

        internal class TestInterceptor4 : TestInterceptor3
        {
            public TestInterceptor4(Func<int> f) : base(f) { }
            public override void Intercept(IInvocation memberInvocation)
            {
                InterceptReceivedCall++;
                // do not call proceed
                memberInvocation.ReturnValue = _f();
            }
        }
    }
}
