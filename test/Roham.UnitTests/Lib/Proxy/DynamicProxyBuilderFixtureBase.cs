using Castle.DynamicProxy;
using System;

namespace Roham.Lib.Proxy
{
    public partial class DynamicProxyBuilderFixtureBase : UnitTestFixture
    {
        public class ClassA
        {
            public virtual void AMethod(Action action)
            {
                action();
            }

            public virtual string AP1 { get; set; }
            public virtual ClassB AP2 { get; set; }
        }

        public class ClassSubA : ClassA
        {
            public virtual void SubAMethod(Action action)
            {
                action();
            }

            public virtual int SubAP1 { get; set; }
        }

        public class ClassB
        {
            public virtual int P1 { get; set; }
            public virtual string P2 { get; set; }

            public virtual int PowerTwo(int input)
            {
                return input * input;
            }

            public override string ToString()
            {
                return string.Format(@"{0} [P1: {1}, P2: {2}]", GetType().Name, P1, P2);
            }
        }

        public class InterceptorA : IInterceptor
        {
            public int InterceptReceivedCall { get; protected set; }

            public void Intercept(IInvocation invk)
            {
                InterceptReceivedCall++;

                Console.Write("{0} called", GetType().Name);
                Console.Write(" InvocationTarget: {0}, Target:{1}", invk.InvocationTarget.GetType().Name, invk.TargetType.Name);
                Console.WriteLine(" MethodInvocationTarget: {0}, Method:{1}", invk.MethodInvocationTarget.Name, invk.Method.Name);

                invk.Proceed();

                Console.WriteLine("{0} ReturnValue: {1}", GetType().Name, invk.ReturnValue == null ? "Null" : invk.ReturnValue);
            }
        }

        public class InterceptorB : InterceptorA { }

        public interface IMixinA
        {
            int MixinAProperty { get; }
            void MixinAMethod(Action action);
        }

        public interface IMixinB
        {
            int MixinBProperty { get; set; }
            void MixinBMethod(Action action);
        }

        public class MixinA : IMixinA
        {
            public MixinA(int mixinAProperty)
            {
                MixinAProperty = mixinAProperty;
            }

            public virtual int MixinAProperty { get; private set; }

            public virtual void MixinAMethod(Action action)
            {
                action();
            }
        }

        public class MixinB : IMixinB
        {
            public virtual int MixinBProperty { get; set; }
            public virtual void MixinBMethod(Action action)
            {
                action();
            }
        }

        public class MixinAB : IMixinA, IMixinB
        {
            public MixinAB(int mixinAProperty)
            {
                MixinAProperty = mixinAProperty;
            }

            public virtual int MixinAProperty { get; private set; }
            public virtual int MixinBProperty { get; set; }
            public virtual void MixinAMethod(Action action)
            {
                action();
            }

            public virtual void MixinBMethod(Action action)
            {
                action();
            }
        }

        public interface IClassC
        {
            void CMethod(Action action);
            int CP1 { get; set; }
        }

        public class ClassC : IClassC
        {
            public void CMethod(Action action)
            {
                action();
            }

            public int CP1 { get; set; }
        }
    }
}
