using System.Web;
using System.Web.Routing;
using Roham.Domain.Permissions;

namespace Roham.Web.Mvc.Routes
{
    public class ValidPageNameRouteConstraint : IRouteConstraint
    {
        public readonly static string PageParameterName = "page";
        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var siteName = (values[ValidSiteNameRouteConstraint.SiteParameterName] ?? "").ToString().Trim();
            var zoneName = (values[ValidZoneNameRouteConstraint.ZoneParameterName] ?? "").ToString().Trim();
            var pageName = (values[parameterName] ?? "").ToString().Trim();
            var currentUserName = httpContext.Request.IsAuthenticated ? (httpContext?.User?.Identity?.Name) : null;
            string errorMsg = "";

            var nameValidator = RohamDependencyResolver.Current.Resolve<IPermissionChecker>();
            return nameValidator.PageAccessible(siteName, zoneName, pageName, currentUserName, out errorMsg);
        }
    }
}
