using System;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class InterfaceProxyWithTargetInterfaceTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithTargetInterface_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var cTarget1 = new ClassC { CP1 = 5 };
                var cTarget2 = new ClassC { CP1 = 15 };
                var interceptor1 = new Inteceptor1();
                IClassC proxy = subjectBuilder
                    .ForInterface<IClassC>()
                    .WithTargetInterface(cTarget1)
                    .AddInterceptor(interceptor1)
                    .Build();

                // When
                interceptor1.ChangeProxyTarget(cTarget2);
                interceptor1.ChangeInvocationTarget(cTarget2);

                int aValue = 0;
                proxy.CMethod(() => aValue = 10);
                var ret1_CP1 = proxy.CP1;
                proxy.CP1 = 17;
                var ret2_CP1 = proxy.CP1;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(4, interceptor1.InterceptReceivedCall);
                Assert.AreEqual(10, aValue);
                Assert.AreEqual(15, ret1_CP1);
                Assert.AreEqual(17, ret2_CP1);
            }
        }

        internal class Inteceptor1 : IInterceptor, IChangeProxyTarget
        {
            private object _proxyTarget;
            private object _invocationTarget;

            public int InterceptReceivedCall { get; protected set; }

            public void Intercept(IInvocation invk)
            {
                InterceptReceivedCall++;

                (invk as IChangeProxyTarget).ChangeProxyTarget(_proxyTarget);
                (invk as IChangeProxyTarget).ChangeInvocationTarget(_invocationTarget);

                Console.Write("{0} called", GetType().Name);
                Console.Write(" NewInvocationTarget: {0}, NewTarget:{1}", invk.InvocationTarget.GetType().Name, invk.TargetType.Name);
                Console.WriteLine(" MethodInvocationTarget: {0}, Method:{1}", invk.MethodInvocationTarget.Name, invk.Method.Name);

                invk.Proceed();

                Console.WriteLine("{0} ReturnValue: {1}", GetType().Name, invk.ReturnValue == null ? "Null" : invk.ReturnValue);
            }

            public void ChangeInvocationTarget(object target)
            {
                _invocationTarget = target;
            }

            public void ChangeProxyTarget(object target)
            {
                _proxyTarget = target;
            }
        }
    }
}
