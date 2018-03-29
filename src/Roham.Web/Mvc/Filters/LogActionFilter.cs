using System.Linq;
using System.Web.Mvc;
using Roham.Lib.Logger;
using System.Web.Http.Controllers;

namespace Roham.Web.Mvc.Filters
{
    public class LogActions : System.Web.Mvc.ActionFilterAttribute
    {
        private static ILogger Logger = LoggerFactory.GetLogger("ActionLogger");

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Logger.IsDebugEnabled)
            {
                return;
            }

            var routeData = filterContext.RouteData;
            var className = routeData.Values["controller"];
            var methodName = routeData.Values["action"];
            var methodParams = "";
            if (filterContext.ActionParameters != null)
            {
                methodParams = string.Join(",", filterContext.ActionParameters.Select(p => $"{p.Key}:{p.Value ?? "null"}"));
            }
            Logger.Debug($"+{className}::{methodName}({methodParams})");

        }
    }

    public class ApiLogActions : System.Web.Http.Filters.ActionFilterAttribute
    {
        private static ILogger Logger = LoggerFactory.GetLogger("ApiActionLogger");

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!Logger.IsDebugEnabled)
            {
                return;
            }

            var routeData = actionContext.ControllerContext.RouteData;
            var className = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            var methodParams = "";
            if (actionContext.ActionArguments != null)
            {
                methodParams = string.Join(",", actionContext.ActionArguments.Select(p => $"{p.Key}:{p.Value ?? "null"}"));
            }
            Logger.Debug($"+{className}::{methodName}({methodParams})");

        }
    }
}
