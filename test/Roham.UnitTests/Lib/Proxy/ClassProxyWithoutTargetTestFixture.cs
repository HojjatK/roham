using Castle.DynamicProxy;
using NUnit.Framework;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase
    {
        [TestFixture]
        [Category("UnitTests.DynamicProxy")]
        internal class ClassProxyWithoutTargetTestFixture
        {
            [Test]
            public void GivenBuiltProxyWithInterceptor_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aInterceptor = new InterceptorA();
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithoutTraget()
                    .AddInterceptor(aInterceptor)
                    .Build();

                // When
                int aValue = 0;
                proxy.AMethod(() => aValue = 10);
                proxy.AP1 = "a1";
                proxy.AP2 = new ClassB { P1 = 1, P2 = "2" };

                var retAP1 = proxy.AP1;
                var retAP2 = proxy.AP2;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(5, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(10, aValue);
                Assert.AreEqual("a1", retAP1);
                Assert.AreEqual(1, retAP2.P1);
                Assert.AreEqual("2", retAP2.P2);
            }

            [Test]
            public void GivenBuiltProxyWithInterceptors_WhenMemberIsCalled_ThenMemberIsIntercepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aInterceptor = new InterceptorA();
                var bInterceptor = new InterceptorB();
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithoutTraget()
                    .AddInterceptors(aInterceptor, bInterceptor)
                    .Build();

                // When
                proxy.AP1 = "a1";
                var retAP1 = proxy.AP1;

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(2, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(2, bInterceptor.InterceptReceivedCall);
                Assert.AreEqual("a1", retAP1);
            }

            [Test]
            public void GivenBuiltProxyWithMixin_WhenMemberIsCalled_ThenMixinMemberIsIntecepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aInterceptor = new InterceptorA();
                var abMixin = new MixinAB(mixinAProperty: 4);
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithoutTraget()
                    .AddInterceptor(aInterceptor)
                    .WithMixin(abMixin)
                    .Build();

                // When
                int aValue = 0;
                proxy.AMethod(() => aValue += 5);
                (proxy as IMixinA).MixinAMethod(() => aValue += 5);
                (proxy as IMixinB).MixinBMethod(() => aValue += 5);

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(3, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual(4, (proxy as IMixinA).MixinAProperty);
                Assert.AreEqual(15, aValue);
            }

            [Test]
            public void GivenBuiltProxyWithTwoMixins_WhenMemberIsCalled_ThenMixinMembersAreIntecepted()
            {
                var subjectBuilder = new DynamicProxyBuilder();

                // Given
                var aInterceptor = new InterceptorA();
                var aMixin = new MixinA(mixinAProperty: 4);
                var bMixin = new MixinB();
                var proxy = subjectBuilder
                    .ForClass<ClassA>()
                    .WithoutTraget()
                    .AddInterceptor(aInterceptor)
                    .WithMixin(aMixin)
                    .WithMixin(bMixin)
                    .Build();

                // When
                int aValue = 0;
                proxy.AMethod(() => aValue += 2); // i1
                proxy.AP1 = "a1"; // i2
                proxy.AP2 = new ClassB { P1 = 11, P2 = "22" }; // i3
                (proxy as IMixinA).MixinAMethod(() => aValue += 4); // i4
                (proxy as IMixinB).MixinBMethod(() => aValue += 8); // i5
                (proxy as IMixinB).MixinBProperty = 33; // i6

                var retAP1 = proxy.AP1; // i7
                var retAP2 = proxy.AP2; // i8
                var retAP2_P1 = retAP2.P1;
                var retAP2_P2 = retAP2.P2;
                var retAP2_PowerTwo = proxy.AP2.PowerTwo(4); // i9
                var retMixinAProperty = (proxy as IMixinA).MixinAProperty; // i10
                var retMixinBProperty = (proxy as IMixinB).MixinBProperty; // i11

                // Then
                Assert.IsTrue(ProxyUtil.IsProxy(proxy), "object is not a proxy");
                Assert.AreEqual(11, aInterceptor.InterceptReceivedCall);
                Assert.AreEqual("a1", retAP1);
                Assert.AreEqual(11, retAP2_P1);
                Assert.AreEqual("22", retAP2_P2);
                Assert.AreEqual(16, retAP2_PowerTwo);
                Assert.AreEqual(4, retMixinAProperty);
                Assert.AreEqual(33, retMixinBProperty);
            }
        }
    }
}
