using Roham.Lib.Domain;
using Roham.Domain.Entities.Sites;
using Roham.Contracts.Dtos;
using Roham.Lib.Ioc;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class SiteMapper : IEntityMapper<SiteDto, Site>
    {   
        public SiteDto Map(Site site)
        {
            if (site == null)
            {
                return null;
            }

            string siteOwner = "";
            if (site.Owner != null)
            {
                siteOwner = site.Owner.Person != null 
                    ? site.Owner.Person.GetFullName() 
                    : site.Owner.Organisation != null ? site.Owner.Organisation.Name : site.Owner.UserName;
            }
            return new SiteDto
            {
                Uid = site.Uid.ToString(),
                Id = site.Id,
                Name = site.Name,
                Title = site.Title,
                Description = site.Description,
                IsActive = site.IsActive,
                IsDefault = site.IsDefault,
                IsPublic = !site.IsPrivate,
                SiteOwner = siteOwner,
            };
        }
    }
}
