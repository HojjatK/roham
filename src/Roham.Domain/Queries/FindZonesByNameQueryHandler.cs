/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using System.Collections.Generic;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Queries;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindZonesByNameQueryHandler : AbstractQueryHandler<FindZonesByNameQuery, List<ZoneDto>>
    {
        public FindZonesByNameQueryHandler(
          IPersistenceUnitOfWorkFactory uowFactory,
          IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override List<ZoneDto> OnHandle(FindZonesByNameQuery query)
        {
            var zonesDto = new List<ZoneDto>();
            var zoneMapper = _entityMapperFactory.Create<ZoneDto, Zone>();

            using (var uow = _uowFactory.CreateReadOnly())
            {
                var zoneName = query.ZoneName;
                var siteName = query.SiteName;

                List<Zone> zones;
                if (!string.IsNullOrWhiteSpace(siteName))
                {
                    zones = uow.Context.Query<Zone>()
                                       .Where(z => z.Name == zoneName && z.Site.Name == siteName)
                                       .ToList();                    
                }
                else
                {
                    zones = uow.Context.Query<Zone>()
                                          .Where(z => z.Name == zoneName)
                                          .ToList();
                }

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