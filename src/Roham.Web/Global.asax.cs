using Roham.Domain.Exceptions;
using Roham.Domain.Services;
using Roham.Lib.Domain.Cache;
using Roham.Lib.Logger;
using System;
using System.Net;
using System.Web;

namespace Roham.Web
{
    public class Global : HttpApplication
    {
        private readonly static ILogger Log = LoggerFactory.GetLogger<Global>();

        void Application_Start(object sender, EventArgs e)
        {   
        }

        void Application_Error(object sender, EventArgs e)
        {
            var exp = Server.GetLastError();
            if (exp != null)
            {
                Log.Error("An Unhandled error happened", exp);

                bool isAjax = "XMLHttpRequest".Equals(Request.Headers["X-Requested-With"], StringComparison.OrdinalIgnoreCase);
                if (isAjax)
                {   
                    RohamException rohamExp = exp as RohamException;
                    Server.ClearError();
                    Response.ClearContent();
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Response.Write(rohamExp != null ? rohamExp.DisplayMessage : exp.Message);
                    return;
                }
                else
                {   
                    var requestPath = Request.Path != null ? Request.Path.ToLower() : "";
                    if (!requestPath.StartsWith("error") && !requestPath.StartsWith("/error"))
                    {
                        ICacheService cacheService;
                        if (Request.IsLocal && TryResolveCacheService(out cacheService))
                        {   
                            var errorCode = Guid.NewGuid().ToString();                            
                            cacheService.MemoryCache.Set(errorCode, "Unhandled exception", TimeSpan.FromMinutes(1));
                            cacheService.MemoryCache.Set($"{errorCode}-exp", exp.ToString(), TimeSpan.FromMinutes(1));
                            Response.Redirect($"/error?code={errorCode}");
                        }
                        else
                        {
                            Response.Redirect("/error");
                        }
                    }
                    Server.ClearError();
                }
            }
        }

        private bool TryResolveCacheService(out ICacheService cacheService)
        {
            cacheService = null;
            try
            {
                cacheService = RohamDependencyResolver.Current.Resolve<ICacheService>();
                return true;
            }
            catch(Exception ex)
            {
                Log.Error("CacheService could no be resolved", ex);
                return false;
            }
        }
    }
}