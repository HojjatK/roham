using System.Web.Mvc;
using Roham.Web.Mvc.Filters;

namespace Roham.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [LogActions]
    public class HomeController : Controller
    {
        [Route("")]        
        public ActionResult Index()
        {
            return View("AdminPage");
        }
    }
}