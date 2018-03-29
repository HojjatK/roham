using System.Linq;
using System.Collections.Generic;
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Queries;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindNavigationQueryHandler : AbstractQueryHandler<FindNavigationQuery, NavigationDto>
    {
        public FindNavigationQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory, 
            IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override NavigationDto OnHandle(FindNavigationQuery query)
        {   
            var userName = query.UserName;
            var result = new NavigationDto();

            using (var uow = _uowFactory.CreateReadOnly())
            {
                var appFunctions = uow.Context
                    .Query<AppFunction>()
                    .Where(a => true)
                    .ToList();                    

                var roles = uow.Context
                    .Query<User>()
                    .Select(u => u.Roles);

                var userRoleNames = uow.Context
                    .Query<User>()
                    .Where(u => u.UserName == userName)
                    .SelectMany(u => u.Roles)
                    .Select(r => r.Name)
                    .ToList();

                var items = GetNavItems(appFunctions, userRoleNames);
                result.NavItems = FilterInAccessibleItems(items);

                uow.Complete();
            }

            return result;
        }

        private List<NavItemDto> GetNavItems(
            IList<AppFunction> appFunctions,
            IList<string> userRoleNames)
        {
            var result = new List<NavItemDto>();
            var appFunctionsDict = appFunctions.ToDictionary(f => f.Key);
            var rootFuncs = appFunctions.Where(f => f.Parent == null).ToList();
            foreach(var appFunc in rootFuncs)
            {
                var navItem = ConvertAppFunc(appFunc, userRoleNames, appFunctionsDict);
                result.Add(navItem);
                navItem.SubItems = GetChildNavItems(appFunc, userRoleNames, appFunctionsDict);
            }
            return result;
        }

        private List<NavItemDto> GetChildNavItems(AppFunction parentAppFunc, IList<string> userRoleNames, IDictionary<FunctionKeys, AppFunction> appFunctionsDict)
        {
            var result = new List<NavItemDto>();
            var appFuncs = appFunctionsDict.Values.Where(f => f.Parent != null && f.Parent.Id == parentAppFunc.Id).ToList();
            foreach(var appFunc in appFuncs)
            {
                var navItem = ConvertAppFunc(appFunc, userRoleNames, appFunctionsDict);
                result.Add(navItem);
                navItem.SubItems = GetChildNavItems(appFunc, userRoleNames, appFunctionsDict);                
            }
            return result;
        }
        
        private NavItemDto ConvertAppFunc(AppFunction appFunc, IList<string> userRoleNames, IDictionary<FunctionKeys, AppFunction> appFunctions)
        {
            var navItem = new NavItemDto
            {
                Key = (int)appFunc.Key,
                Name = appFunc.Name,
                Title = appFunc.Title,
                Level = GetAppFuncLevel(appFunc),
                Parent = appFunc.Parent != null ? appFunc.Parent.Name : "",
                IsAccessible = CurrentUserHasAccess(appFunc.Key, userRoleNames, appFunctions),
            };
            return navItem;
        }

        //private NavItemDto CreateNavItemFromAppFunction(
        //    IDictionary<FunctionKeys, AppFunction> appFunctions,
        //    IList<string> userRoleNames,
        //    FunctionKeys functionKey,            
        //    string route,
        //    string module,
        //    bool nav,
        //    string icon,
        //    List<NavItemDto> subItems = null)
        //{
        //    var appFunc = appFunctions[functionKey];
        //    var navItem = new NavItemDto
        //    {
        //        Key = (int)appFunc.Key,
        //        Name = appFunc.Name,
        //        Title = appFunc.Title,
        //        Route = route,
        //        ModuleId = module,
        //        IsNav = nav,
        //        Icon = icon,
        //        Level = GetAppFuncLevel(appFunc),
        //        Parent = appFunc.Parent != null ? appFunc.Parent.Name : "",
        //        IsAccessible = CurrentUserHasAccess(functionKey, userRoleNames, appFunctions),
        //    };

        //    if (subItems != null)
        //    {
        //        navItem.SubItems = subItems;
        //    }
        //    return navItem;
        //}        

        private int GetAppFuncLevel(AppFunction appFunc)
        {
            int level = 0;
            if (appFunc.Parent == null)
            {
                return level;
            }
            return GetAppFuncLevel(appFunc.Parent) + 1;
        }

        private bool CurrentUserHasAccess(
            FunctionKeys functionKey,
            IList<string> userRoleNames,
            IDictionary<FunctionKeys, AppFunction> appFunctionDict)
        {

            AppFunction appFunc;
            if (!appFunctionDict.TryGetValue(functionKey, out appFunc))
            {
                return false;
            }            
            return appFunc.GetRoles().Any(r => userRoleNames.Contains(r.Name));
        }

        private List<NavItemDto> FilterInAccessibleItems(List<NavItemDto> items)
        {
            var filtered = new List<NavItemDto>();
            foreach (var item in items)
            {
                if (item.IsAccessible)
                {
                    item.SubItems.RemoveAny(item.SubItems.Where(s => !s.IsAccessible).ToList());
                    filtered.Add(item);
                }
            }
            return filtered;
        }
    }
}
