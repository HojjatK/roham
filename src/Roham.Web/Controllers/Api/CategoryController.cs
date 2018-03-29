using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Roham.Contracts.Commands.Category;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Posts;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Entities.Security;

namespace Roham.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/category")]
    public class CategoryController : ApiControllerBase
    {
        public CategoryController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher) { }

        [HttpGet]
        [Route("")]
        public List<CategoryDto> GetCategories()
        {
            return QueryExecutor.Execute(new FindAllQuery<CategoryDto, Category>());
        }

        [HttpGet]
        [Route("tree/{siteId:long}")]
        public List<CategoryNodeDto> GetCategoryTree(long? siteId = null)
        {
            var result = new List<CategoryNodeDto>();

            var allCategories = GetCategories();
            var categoriesLookup = allCategories.GroupBy(c => c.SiteId).ToDictionary(g => g.Key, g => g.ToList());

            var sites = QueryExecutor.Execute(new FindAllQuery<SiteDto, Site>());
            if (siteId.HasValue)
            {
                sites = sites.Where(s => s.Id == siteId.Value).ToList();
            }
            foreach(var site in sites)
            {   
                var siteTitle = site.Title;                
                if (categoriesLookup.ContainsKey(site.Id))
                {
                    var items = new List<CategoryNodeDto>();
                    var allSiteCategories = categoriesLookup[site.Id];
                    foreach (var rootCategoryDto in allSiteCategories.Where(c => c.ParentId == null))
                    {
                        var rootCategoryNode = ConvertToNode(rootCategoryDto, allSiteCategories);
                        items.Add(rootCategoryNode);
                    }
                    result.AddRange(items);
                }
            }            

            return result;
        }

        private CategoryNodeDto ConvertToNode(CategoryDto source, IList<CategoryDto> allCategories)
        {
            var node = new CategoryNodeDto
            {
                Id = source.Id,
                ParentId = source.ParentId,
                ParentName = source.ParentName, 
                SiteId = source.SiteId,
                SiteTitle = source.SiteTitle,
                Name = source.Name,
                IsPublic = source.IsPublic,
                Description = source.Description,
                Children = new List<CategoryNodeDto>(),
            };
            var sourceChildren = allCategories.Where(c => c.ParentId != null && c.ParentId == source.Id).ToList();
            foreach(var child in sourceChildren)
            {
                node.Children.Add(ConvertToNode(child, allCategories));
            }
            return node;
        }

        [HttpGet]
        [Route("{id:long}")]
        public CategoryDto GetCategory(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<CategoryDto, Category>(id));
        }

        [HttpPost]
        [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
        [Route("")]
        public ResultDto CreateCategory(CategoryDto newCategory)
        {
            return Result(() =>
            {
                var command = new AddCategoryCommand
                {   
                    Name = newCategory.Name,
                    Description = newCategory.Description,                    
                    ParentId = newCategory.ParentId,
                    SiteId = newCategory.SiteId,
                    IsPublic = newCategory.IsPublic,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]
        [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
        [Route("{id:long}")]
        public ResultDto UpdateCategory(long id, CategoryDto category)
        {
            return Result(() =>
            {
                var command = new UpdateCategoryCommand
                {
                    CategoryId = id,
                    Description = category.Description,
                    ParentCategoryId = category.ParentId,
                    IsPublic = category.IsPublic,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
        [Route("{id:long}")]
        public ResultDto DeleteCategory(long id)
        {
            return Result(() => {
                var command = new DeleteCategoryCommand
                {
                    CategoryId = id,
                };
                CommandDispatcher.Send(command);
            });
        }
    }
}