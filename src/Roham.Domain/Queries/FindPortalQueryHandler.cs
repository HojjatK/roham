using System.Linq;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Domain.Exceptions;
using Roham.Contracts.Queries;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindPortalQueryHandler : AbstractQueryHandler<FindPortalQuery, PortalDto>
    {
        public FindPortalQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) {}

        protected override PortalDto OnHandle(FindPortalQuery query)
        {
            PortalDto portalDto = null;
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var portal = uow.Context
                    .Query<Portal>()
                    .SingleOrDefault();
                if (portal == null)
                {
                    throw new EntityNotFoundException("Portal not found");
                }

                portalDto = _entityMapperFactory.Create<PortalDto, Portal>().Map(portal);

                uow.Complete();
            }

            return portalDto;
        }
    }
}
