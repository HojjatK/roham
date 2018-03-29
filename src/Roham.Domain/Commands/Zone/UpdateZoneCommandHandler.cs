using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Zone;
using Roham.Lib.Ioc;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Zone
{
    [AutoRegister]
    public class UpdateZoneCommandHandler : AbstractCommandHandler<UpdateZoneCommand>
    {   
        public UpdateZoneCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateZoneCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);
                var zone = uow.Context.FindById<Entities.Sites.Zone>(command.ZoneId);

                zone.Name = command.Name;
                zone.Title = command.Title;
                zone.Description = command.Description;
                zone.IsActive = command.IsActive;
                zone.IsPrivate = !command.IsPublic;

                uow.Context.Update(zone);

                uow.Complete();
            }
        }

        private void Validate(UpdateZoneCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            var siteId = command.SiteId;
            var zoneId = command.ZoneId;
            PageName zoneName = command.Title;

            if (uow.Context.Query<Entities.Sites.Zone>().Any(s => s.Name == zoneName && s.Site.Id == siteId && s.Id != zoneId))
            {
                throw new ValidationException($"Zone with '{zoneName}' name already exist");
            }
        }
    }
}
