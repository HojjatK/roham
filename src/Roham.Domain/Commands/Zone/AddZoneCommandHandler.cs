using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Sites;
using Roham.Contracts.Commands.Zone;
using Roham.Lib.Ioc;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Zone
{
    [AutoRegister]
    public class AddZoneCommandHandler : AbstractCommandHandler<AddZoneCommand>
    {
        public AddZoneCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddZoneCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                ZoneTypeCodes zoneTypeCode;
                if (!Enum.TryParse(command.ZoneType, out zoneTypeCode))
                {
                    throw new ArgumentOutOfRangeException($"Zone type: {command.ZoneType} is not valid");
                }

                Validate(command, uow);
                long siteId = command.SiteId;
                var site = uow.Context.
                    FindById<Entities.Sites.Site>(siteId);

                var zone = new Entities.Sites.Zone
                {
                    Name = command.Name,
                    Title = command.Title,                    
                    ZoneType = zoneTypeCode,
                    IsActive = command.IsActive,
                    IsPrivate = !command.IsPublic,
                    Site = site,
                    Description = command.Description,
                };
                uow.Context.Add(zone);                

                uow.Complete();
            }
        }

        private void Validate(AddZoneCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            var siteId = command.SiteId;
            PageName newZoneName = command.Title;
            if (uow.Context.Query<Entities.Sites.Zone>().Any(s => s.Name == newZoneName && s.Site.Id == siteId))
            {
                throw new ValidationException($"Zone with '{newZoneName}' name already exist");
            }
        }
    }
}
