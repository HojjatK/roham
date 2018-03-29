using System.Web;
using System.Web.Routing;
using Roham.Domain.Permissions;

namespace Roham.Web.Mvc.Routes
{
    public class ValidSiteNameRouteConstraint : IRouteConstraint
    {
        public readonly static string SiteParameterName = "site";

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var siteName = (values[parameterName] ?? "").ToString().Trim();
            var currentUserName = httpContext.Request.IsAuthenticated ? (httpContext?.User?.Identity?.Name) : null;
            string errorMsg = "";

            var nameValidator = RohamDependencyResolver.Current.Resolve<IPermissionChecker>();
            return nameValidator.SiteAccessible(siteName, currentUserName, out errorMsg);
        }
    }
}
