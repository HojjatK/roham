using System;
using IOC = Roham.Lib.Ioc;
using AF = Autofac;

namespace Roham.Ioc.Autofac
{
    public class AutofacIocFactory
    {
        public static IOC.IRegistrator CreateRegistrator(AF.ContainerBuilder containerBuilder)
        {
            return new RegistratorImpl(containerBuilder);
        }

        public static Type GetLifetimeScopeType()
        {
            return typeof(LifetimeScopeImpl);
        }
    }
}
