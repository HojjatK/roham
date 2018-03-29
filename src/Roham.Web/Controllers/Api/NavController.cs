using System.Web.Http;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;

namespace Roham.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/nav")]    
    public class NavController : ApiControllerBase
    {
        public NavController(IQueryExecutor queryExecutor) : base(queryExecutor, null) {}

        [HttpGet]
        [Route("nav-items")]
        public NavigationDto GetNav()
        {
            var nav = QueryExecutor.Execute(new FindNavigationQuery { UserName = User?.Identity?.Name });
            nav.Title = WebAppInfo.Name;
            return nav;
        }
    }
}
