using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class InterfaceProxyWithTargetTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithTarget_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var cTarget = new ClassC { CP1 = 5 };
                var aInterceptor = new InterceptorA();
                var bInterceptor = new InterceptorB();
                IClassC proxy = subjectBuilder
                    .ForInterface<IClassC>()
                    .WithTarget(cTarget)
                    .AddInterceptors(aInterceptor, bInterceptor)
                    .Build();

                // When
                int aValue = 0;
                proxy.CMethod(() => aValue = 10);
                var ret1_CP1 = proxy.CP1;
                proxy.CP1 = 7;
                var ret2_CP1 = proxy.CP1;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(4, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(4, bInterceptor.InterceptReceivedCall);
                Assert.AreEqual(10, aValue);
                Assert.AreEqual(5, ret1_CP1);
                Assert.AreEqual(7, ret2_CP1);
            }
        }
    }
}
