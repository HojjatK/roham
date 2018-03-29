using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using System.Collections.Generic;
using System.Linq;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindSitesQueryHandler : AbstractQueryHandler<FindAllQuery<SiteDto, Site>, List<SiteDto>>
    {   
        public FindSitesQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override List<SiteDto> OnHandle(FindAllQuery<SiteDto, Site> query)
        {
            var result = new List<SiteDto>();
            var siteMapper = _entityMapperFactory.Create<SiteDto, Site>();
            var zoneMapper = _entityMapperFactory.Create<ZoneDto, Zone>();

            using (var uow = _uowFactory.CreateReadOnly())
            {
                var sites = uow.Context.All<Site>();

                foreach (var site in sites)
                {
                    var siteDto = siteMapper.Map(site);

                    long siteId = site.Id;
                    var zones = uow.Context
                        .Query<Zone>()
                        .Where(z => z.Site.Id == siteId)
                        .ToList();

                    siteDto.Zones = new List<ZoneDto>();
                    siteDto.Zones.AddRange(zones.Select(z => zoneMapper.Map(z)));

                    result.Add(siteDto);
                }

                uow.Complete();
            }

            return result;
        }
    }
}
