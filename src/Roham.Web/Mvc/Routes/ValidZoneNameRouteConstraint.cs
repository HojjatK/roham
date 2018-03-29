using System.Web;
using System.Web.Routing;
using Roham.Domain.Permissions;

namespace Roham.Web.Mvc.Routes
{
    public class ValidZoneNameRouteConstraint : IRouteConstraint
    {
        public readonly static string ZoneParameterName = "zone";

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var siteName = (values[ValidSiteNameRouteConstraint.SiteParameterName ?? ""]).ToString().Trim();
            var zoneName = (values[parameterName] ?? "").ToString().Trim();            
            var currentUserName = httpContext.Request.IsAuthenticated ? (httpContext?.User?.Identity?.Name) : null;
            string errorMsg = "";

            var nameValidator = RohamDependencyResolver.Current.Resolve<IPermissionChecker>();
            return nameValidator.ZoneAccessible(siteName, zoneName, currentUserName, out errorMsg);
        }
    }
}
