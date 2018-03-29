using AF = Autofac;
using Autofac;
using Roham.Caching;
using Roham.Data;
using Roham.Domain.Services;

namespace Roham.Web.IocModules
{
    //public class CachingModule : AF.Module
    //{
    //    private readonly int cacheDuration;
    //    public CachingModule(int cacheDurationInMinutes)
    //    {
    //        cacheDuration = cacheDurationInMinutes;
    //    }
    //    protected override void Load(AF.ContainerBuilder builder)
    //    {
    //        base.Load(builder);

    //        builder
    //           .RegisterInstance(new CacheProvider())
    //           .As<ICacheProvider>()
    //           .SingleInstance();

    //        builder
    //           .RegisterType<CacheService>()
    //           .As<ICacheService>()
    //           .SingleInstance();
    //    }
    //}
}