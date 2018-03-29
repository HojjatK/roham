using System.Linq;
using System.Web.Mvc;
using Roham.Contracts.Queries;
using Roham.Lib.Domain.CQS.Query;
using Roham.Web.Mvc.Filters;
using Roham.Web.ViewModels;
using System.Collections.Generic;

namespace Roham.Web.Controllers
{
    [RoutePrefix("")]
    [LogActions]
    public class HomeController : Controller
    {
        private IQueryExecutor _queryExecutor;

        public HomeController(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        [AllowAnonymous]
        [Route("")]        
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("{site:site}")]
        public ActionResult Site(string site)
        {   
            return View();
        }

        [AllowAnonymous]
        [Route("{site:site}/{zone:zone}")]
        public ActionResult Zone(string site, string zone)
        {   
            var vm = new ZoneViewModel
            {
                Posts = new List<PostViewModel>()
            };
            var zoneDto = _queryExecutor.Execute(new FindZonesByNameQuery(zone, site)).FirstOrDefault();
            if (zoneDto != null)
            {
                vm.Title = zoneDto.Name;
            }
            var postDtos = _queryExecutor.Execute(new FindPostSummariesQuery(site, zone));
            var styles = new List<string>();
            foreach (var postDto in postDtos)
            {
                vm.Posts.Add(new PostViewModel
                {
                    Uri = $"{postDto.SiteName}/{postDto.ZoneName}/{postDto.Name}",
                    Title = postDto?.Title,
                    DisplayDate = postDto.DisplayDate,
                    Tags = postDto.TagsCommaSeparated,
                    CommentsCount = postDto.CommentsCount,
                    HtmlContent = postDto?.Content,
                });
                if (postDto?.Links != null)
                {   
                    foreach (var link in postDto?.Links.Where(l => string.Equals(l.Type, "text/css", System.StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!styles.Contains(link.Ref))
                        {
                            styles.Add(link.Ref);
                        }
                    }
                }
            }
            ViewBag.Styles = styles;
            return View(vm);
        }

        [AllowAnonymous]
        [Route("{site:site}/{zone:zone}/{page:page}")]
        public ActionResult Post(string site, string zone, string page)
        {
            var postDto = _queryExecutor.Execute(new FindPostByNameQuery(site, zone, page));
            if (postDto == null)
            {
                ModelState.AddModelError("pagenotfound", "Page not found");
            }
            var styles = new List<string>();
            if (postDto?.Links != null)
            {
                ViewBag.Styles = new List<string>();
                foreach(var link in postDto?.Links.Where(l =>  string.Equals(l.Type, "text/css", System.StringComparison.OrdinalIgnoreCase)))
                {
                    if (!styles.Contains(link.Ref))
                    {
                        styles.Add(link.Ref);
                    }
                }
            }

            var vm = new PostViewModel
            {
                Title = postDto?.Title,
                DisplayDate = postDto.DisplayDate,
                Tags = postDto.TagsCommaSeparated,
                CommentsCount = postDto.CommentsCount,
                HtmlContent = postDto?.Content,
            };

            ViewBag.Styles = styles;
            return View(vm);
        }
    }
}