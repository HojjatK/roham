using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using System.Linq;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindSiteByIdQueryHandler : AbstractQueryHandler<FindByIdQuery<SiteDto, Site>, SiteDto>
    {
        public FindSiteByIdQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) {}

        protected override SiteDto OnHandle(FindByIdQuery<SiteDto, Site> query)
        {
            SiteDto siteDto = null;
            var siteMapper = _entityMapperFactory.Create<SiteDto, Site>();
            var zoneMapper = _entityMapperFactory.Create<ZoneDto, Zone>();

            using (var uow = _uowFactory.CreateReadOnly())
            {
                var site = uow.Context.FindById<Site>(query.Id);
                siteDto = siteMapper.Map(site);

                long siteId = siteDto.Id;
                var zones = uow.Context
                    .Query<Zone>()
                    .Where(z => z.Site.Id == siteId)
                    .ToList();
                siteDto.Zones = zones.Select(z => zoneMapper.Map(z)).ToList();

                uow.Complete();
            }

            return siteDto;
        }
    }
}
