using System.Linq;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Sites;
using Roham.Contracts.Queries;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindUserSitesQueryHandler : AbstractQueryHandler<FindUserSitesQuery, UserSitesAndZonesDto>
    {
        public FindUserSitesQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override UserSitesAndZonesDto OnHandle(FindUserSitesQuery query)
        {
            var userName = query.UserName;
            var result = new UserSitesAndZonesDto();
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var allSites = uow.Context.All<Site>();
                foreach (var s in allSites)
                {
                    if (s.IsPrivate)
                    {
                        if (!s.Users.Any(u => u.UserName == userName))
                        {
                            continue;
                        }
                    }

                    var userSite = new UserSitesAndZonesDto.UserSiteDto
                    {
                        Name = s.Name,
                        IsActive = s.IsActive,
                        IsDefault = s.IsDefault,
                        IsPublic = !s.IsPrivate,
                    };

                    result.Sites.Add(userSite);

                    var siteId = s.Id;
                    var zones = uow.Context
                        .Query<Zone>()
                        .Where(z => z.Site.Id == siteId);
                    foreach (var z in zones)
                    {
                        if (z.IsPrivate && string.IsNullOrEmpty(userName))
                        {
                            continue;
                        }
                        var userZone = new UserSitesAndZonesDto.UserZoneDto
                        {
                            Name = z.Name,
                            IsActive = z.IsActive,
                            Type = z.ZoneType.ToString(),
                            IsPublic = !z.IsPrivate,
                        };
                        userSite.Zones.Add(userZone);
                    }
                }

                uow.Complete();
            }

            return result;
        }
    }
}
