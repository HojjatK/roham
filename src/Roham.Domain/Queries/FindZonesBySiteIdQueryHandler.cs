using System.Linq;
using System.Collections.Generic;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Sites;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Lib.Ioc;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindZonesBySiteIdQueryQueryHandler : AbstractQueryHandler<FindZonesBySiteIdQuery, List<ZoneDto>>
    {
        public FindZonesBySiteIdQueryQueryHandler(
          IPersistenceUnitOfWorkFactory uowFactory,
          IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override List<ZoneDto> OnHandle(FindZonesBySiteIdQuery query)
        {
            var zonesDto = new List<ZoneDto>();
            var zoneMapper = _entityMapperFactory.Create<ZoneDto, Zone>();

            using (var uow = _uowFactory.CreateReadOnly())
            {
                long siteId = query.SiteId;
                var zones = uow.Context.
                    Query<Zone>()
                    .Where(z => z.Site.Id == siteId);
                
                foreach (var zone in zones)
                {
                    zonesDto.Add(zoneMapper.Map(zone));
                }

                uow.Complete();
            }

            return zonesDto;
        }
    }
}
