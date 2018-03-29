using Microsoft.Owin;
using Owin;
using Roham.Data;
using Roham.Domain.Configs;
using Roham.Domain.Services;
using Roham.Lib.Caches;
using Roham.Lib.Domain.Cache;
using Roham.Resources;
using System;

namespace Roham.Web
{
    public partial class Startup
    {
        private void ConfigureMiddlewares(IAppBuilder app, IRohamConfigs rohamConfigsParam, ICacheService cacheService)
        {
            var rohamConfigs = rohamConfigsParam;
            app.Use(async (context, next) =>
            {
                string requestPath = context.Request.Path.Value != null ? context.Request.Path.Value.Trim().ToLower() : "";
                bool isBrowserLink = requestPath.StartsWith("/__browserlink");
                if (!isBrowserLink)
                {   
                    bool isErrorPage = requestPath.StartsWith("/error");

                    if (!isErrorPage)
                    {
                        bool isConfigMissing = rohamConfigs.IsConfigFileMissing;
                        bool appNotInstalled = !rohamConfigs.IsInstalled;
                        bool upgradeRequired = false; // TODO:
                        bool isInstall = requestPath.StartsWith("/admin/install");
                        bool isUpgrade = requestPath.StartsWith("/admin/upgrade");
                        
                        if (isConfigMissing)
                        {
                            RedirectToError(context, cacheService, ErrorMessages.ConfigFileMissing);
                            return;
                        }
                        else if (appNotInstalled && !isInstall)
                        {
                            context.Response.Redirect("/admin/install");
                            return;
                        }
                        else if (upgradeRequired && !requestPath.StartsWith("/admin/upgrade"))
                        {
                            context.Response.Redirect("/admin/upgrade");
                            return;
                        }
                        else
                        {
                            if (!isInstall && !isUpgrade)
                            {
                                // check database is accessible?
                                var dbProvider = RohamDependencyResolver.Current.Resolve<IDatabaseProvider>();
                                string errorMessage;
                                if (!dbProvider.TryConnect(rohamConfigs.ConnectionString, out errorMessage))
                                {
                                    RedirectToError(context, cacheService, ErrorMessages.DatabaseIsDown);
                                    return;
                                }
                            }
                        }
                    }
                }

                try
                {
                    await next();
                }
                catch(Exception ex)
                {
                    Log.Error("An unhadled exception occured.", ex);
                    context.Response.Redirect("/error");
                }
            });
        }

        private void RedirectToError(IOwinContext context, ICacheService cacheService, string errorMessage)
        {
            var errorCode = Guid.NewGuid().ToString();            
            cacheService.MemoryCache.Set(errorCode, errorMessage, TimeSpan.FromMinutes(1));
            context.Response.Redirect($"/error?code={errorCode}");
        }
    }
}
