using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json.Serialization;
using Roham.Lib.Strings;
using Roham.Web.Mvc;
using Roham.Web.Mvc.Routes;

namespace Roham.Web
{
    public partial class Startup
    {
        private void ConfigureRoutes()
        {
            // Web API configuration and services (default is jason)
            GlobalConfiguration.Configure(config => {
                config.Formatters.Remove(config.Formatters.XmlFormatter);
                config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                config.MapHttpAttributeRoutes();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
            });

            // MVC            
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var constraintsResolver = new System.Web.Mvc.Routing.DefaultInlineConstraintResolver();
            constraintsResolver.ConstraintMap.Add(ValidSiteNameRouteConstraint.SiteParameterName, typeof(ValidSiteNameRouteConstraint));
            constraintsResolver.ConstraintMap.Add(ValidZoneNameRouteConstraint.ZoneParameterName, typeof(ValidZoneNameRouteConstraint));
            constraintsResolver.ConstraintMap.Add(ValidPageNameRouteConstraint.PageParameterName, typeof(ValidPageNameRouteConstraint));

            RouteTable.Routes.MapMvcAttributeRoutes(constraintsResolver);
            AreaRegistration.RegisterAllAreas();
        }

        private void ConfigureBinders()
        {
            ModelBinders.Binders.Add(typeof(PageName), new ImplicitAssignmentBinder());
        }
    }
}