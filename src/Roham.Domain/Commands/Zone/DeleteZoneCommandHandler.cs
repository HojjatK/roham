using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Zone;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Zone
{
    [AutoRegister]
    public class DeleteZoneCommandHandler : AbstractCommandHandler<DeleteZoneCommand>
    {
        public DeleteZoneCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteZoneCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var zoneToDelete = uow.Context
                    .FindById<Entities.Sites.Zone>(command.ZoneId);

                Validate(uow, zoneToDelete);
                uow.Context.Remove(zoneToDelete);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, Entities.Sites.Zone zoneToDelete)
        {   
            long zoneId = zoneToDelete.Id;            

            bool hasPosts = uow.Context
                .Query<Entities.Posts.Post>()
                .Any(e => e.Zone.Id == zoneId);

            if (hasPosts)
            {
                throw new ValidationException("Zone has posts, please delete them first and try again.");
            }
        }
    }
}
