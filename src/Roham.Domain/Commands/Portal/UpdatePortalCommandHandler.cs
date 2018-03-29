using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Portal;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Ioc;
using Roham.Domain.Settings;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Commands.Portal
{
    [AutoRegister]
    public class UpdatePortalCommandHandler : AbstractCommandHandler<UpdatePortalCommand>
    {
        private readonly ISettingsProvider _settingsProvider;

        public UpdatePortalCommandHandler(ISettingsProvider settingsProvider, Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver)
        {
            _settingsProvider = settingsProvider;
        }

        protected override void OnHandle(UpdatePortalCommand command)
        {
            var settings = ConvertTo(command.SettingsDto);
            using (var uow = UowFactory.Create())
            {
                var portal = uow.Context
                    .Query<Entities.Sites.Portal>()
                    .SingleOrDefault();
                if (portal == null)
                {
                    throw new EntityNotFoundException("Portal entity not found");
                }
                portal.Name = command.Name;
                portal.Title = command.Title;
                portal.Description = command.Description;
                uow.Context.Update(portal);

                if (settings != null)
                {
                    _settingsProvider.SaveSettings(uow, settings);
                }

                uow.Complete();
            }
        }

        private PortalSettings ConvertTo(PortalSettingsDto portalSettingsDto)
        {
            if (portalSettingsDto == null)
            {
                return null;
            }
            return new PortalSettings
            {
                SiteId = null,
                StorageProvider = portalSettingsDto.StorageProvider,
                UploadPath = portalSettingsDto.UploadPath,
                StorageConnectionString = portalSettingsDto.StorageConnectionString,
                BlobContainerName = portalSettingsDto.BlobContainerName,
                AdminTheme = portalSettingsDto.AdminTheme
            };
        }
    }
}
