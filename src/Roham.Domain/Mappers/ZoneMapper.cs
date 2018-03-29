using Roham.Lib.Domain;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Ioc;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class ZoneMapper : IEntityMapper<ZoneDto, Zone>
    {
        public ZoneDto Map(Zone zone)
        {
            return new ZoneDto
            {
                Uid = zone.Uid.ToString(),
                Id = zone.Id,
                SiteId = zone.Site.Id,
                SiteTitle = zone.Site.Title,
                SiteName = zone.Site.Name,
                Name = zone.Name,
                Title = zone.Title,
                ZoneType = zone.ZoneType.ToString(),
                IsActive = zone.IsActive,
                IsPublic = !zone.IsPrivate,
                Description = zone.Description,
            };
        }
    }
}
