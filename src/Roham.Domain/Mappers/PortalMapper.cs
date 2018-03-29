using Roham.Lib.Domain;
using Roham.Lib.Domain.Persistence;
using Roham.Data;
using Roham.Domain.Entities.Sites;
using Roham.Contracts.Dtos;
using Roham.Lib.Ioc;
using System;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PortalMapper : IEntityMapper<PortalDto, Portal>
    {
        private readonly Func<IPersistenceConfigs> _persistenceConfigsResolver;

        public PortalMapper(Func<IPersistenceConfigs> persistenceConfigsResolver)
        {
            _persistenceConfigsResolver = persistenceConfigsResolver;
        }

        public PortalDto Map(Portal portal)
        {
            var portalDto = new PortalDto
            {
                Uid = portal.Uid.ToString(),
                Name = portal.Name,
                Title = portal.Title,
                Description = portal.Description,
            };

            var persistenceConfigs = _persistenceConfigsResolver();
            var dbInfo = new DatabaseInfo(persistenceConfigs.DatabaseProvider, persistenceConfigs.ConnectionString);
            portalDto.DatabaseInfo = $"{dbInfo.DbProvider} [{dbInfo.DataSource} : {dbInfo.InitialCatalog}]";

            return portalDto;
        }
    }
}
