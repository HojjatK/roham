using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class ClassProxyWithFilterTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithNoInterceptFilter_WhenMemberIsCalled_ThenMemberShouldNotBeIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aInterceptor = new InterceptorA();
                var subAClassType = typeof(ClassSubA);
                var proxy = subjectBuilder
                    .ForClass<ClassSubA>()
                    .WithoutTraget()
                    .AddInterceptor(aInterceptor)
                    .ShouldNotIntercept(subAClassType, subAClassType.GetMethod("SubAMethod"))
                    .ShouldNotIntercept(subAClassType, subAClassType.GetMethod("set_SubAP1"))
                    .Build();

                // When
                var aValue = 0;
                proxy.AMethod(() => aValue += 5);
                proxy.SubAMethod(() => aValue += 10);
                proxy.SubAP1 = 20;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(1, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(15, aValue);
                Assert.AreEqual(20, proxy.SubAP1);
            }
        }
    }
}
