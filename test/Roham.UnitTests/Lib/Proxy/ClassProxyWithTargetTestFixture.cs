using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class ClassProxyWithTargetTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithTarget_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aTarget = new ClassA();
                aTarget.AP1 = "val1";
                var aInterceptor = new InterceptorA();
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithTarget(aTarget)
                    .AddInterceptor(aInterceptor)
                    .Build();

                // When
                int aValue = 0;
                proxy.AMethod(() => aValue = 10);
                var ret1_AP1 = proxy.AP1;
                proxy.AP1 = "val2";
                var ret2_AP1 = proxy.AP1;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(4, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(10, aValue);
                Assert.AreEqual("val1", ret1_AP1);
                Assert.AreEqual("val2", ret2_AP1);
            }

            [Test]
            public void GivenBuiltProxyWithTargetAndMixin_WhenMemberIsCalled_ThenMixinMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aTarget = new ClassA();
                aTarget.AP1 = "val1";
                var aInterceptor = new InterceptorA();
                var abMixin = new MixinAB(mixinAProperty:2) { MixinBProperty = 3 };
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithTarget(aTarget)
                    .AddInterceptor(aInterceptor)
                    .WithMixin(abMixin)
                    .Build();

                // When
                int aValue = 0;
                proxy.AMethod(() => aValue += 5);
                (proxy as IMixinA).MixinAMethod(() => aValue += 5);
                (proxy as IMixinB).MixinBMethod(() => aValue += 5);

                var ret1_AP1 = proxy.AP1;
                var ret1_MixinAProperty = (proxy as IMixinA).MixinAProperty;
                var ret1_MixinBProperty = (proxy as IMixinB).MixinBProperty;

                proxy.AP1 = "val2";
                (proxy as IMixinB).MixinBProperty = 4;
                var ret2_AP1 = proxy.AP1;
                var ret2_MixinBProperty = (proxy as IMixinB).MixinBProperty;


                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(10, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(15, aValue);
                Assert.AreEqual("val1", ret1_AP1);
                Assert.AreEqual("val2", ret2_AP1);
                Assert.AreEqual(2, ret1_MixinAProperty);
                Assert.AreEqual(3, ret1_MixinBProperty);
                Assert.AreEqual(4, ret2_MixinBProperty);
            }
        }
    }
}
