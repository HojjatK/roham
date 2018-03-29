using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Ioc;
using Roham.Contracts.Queries;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Permissions
{
    public interface IPermissionChecker
    {
        bool SiteAccessible(string siteName, string currentUserName, out string errorMessage);
        bool ZoneAccessible(string siteName, string zoneName, string currentUserName, out string errorMessage);
        bool PageAccessible(string siteName, string zoneName, string pageName, string currentUserName, out string errorMessage);
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PermissionChecker : IPermissionChecker
    {
        private readonly Func<IQueryExecutor> _queryExecutorResolver;
        public PermissionChecker(Func<IQueryExecutor> queryExecutorResolver)
        {
            _queryExecutorResolver = queryExecutorResolver;
        }

        public bool SiteAccessible(string siteName, string currentUserName, out string errorMessage)
        {   
            errorMessage = "";
            if (BlackNameList.SiteNames.Any(s => s.Equals(siteName, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = $"{siteName} is not valid";
                return false;
            }
            
            var queryExecutor = _queryExecutorResolver();
            var site = queryExecutor.Execute(new FindByNameQuery<SiteDto, Site>(siteName));
            if (site == null)
            {
                errorMessage = $"{siteName} site not found";
                return false;
            }

            if (!site.IsPublic)
            {
                var userSitesAndZones = queryExecutor.Execute(new FindUserSitesQuery { UserName = currentUserName });
                if (!userSitesAndZones.ContainsSite(siteName))
                {
                    errorMessage = $"{currentUserName} does not have access to {siteName} site.";
                    return false;
                }
            }
            return true;
        }

        public bool ZoneAccessible(string siteName, string zoneName, string currentUserName, out string errorMessage)
        {
            errorMessage = "";
            if (BlackNameList.ZoneNames.Any(s => s.Equals(zoneName, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = $"{zoneName} is not valid";
                return false;
            }

            var queryExecutor = _queryExecutorResolver();

            var zone = queryExecutor.Execute(new FindZonesByNameQuery(zoneName, siteName)).FirstOrDefault();
            if (zone == null)
            {
                errorMessage = $"{zoneName} zone not found";
                return false;
            }

            if (!zone.IsPublic)
            {
                var userSitesAndZones = queryExecutor.Execute(new FindUserSitesQuery { UserName = currentUserName });
                var userZoneNames = userSitesAndZones.Sites.SelectMany(s => s.Zones).Select(z => z.Name).ToList();
                if (!userZoneNames.Any(z => string.Equals(z, zoneName, StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage = $"{currentUserName} does not have access to {zoneName} zone.";
                    return false;
                }
            }
            return true;            
        }

        public bool PageAccessible(string siteName, string zoneName, string pageName, string currentUserName, out string errorMessage)
        {
            errorMessage = "";
            if (BlackNameList.PageNames.Any(s => s.Equals(pageName, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = $"{pageName} is not valid";
                return false;
            }
            return true;

            // TODO: check user has access to page
        }
    }
}
