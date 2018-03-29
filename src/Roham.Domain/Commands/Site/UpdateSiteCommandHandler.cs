using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Site;
using Roham.Lib.Ioc;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Site
{
    [AutoRegister]
    public class UpdateSiteCommandHandler : AbstractCommandHandler<UpdateSiteCommand>
    {
        public UpdateSiteCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateSiteCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);
                var site = uow.Context.FindById<Entities.Sites.Site>(command.SiteId);

                site.Name = command.Name;
                site.Title = command.SiteTitle;
                site.Description = command.Description;
                site.IsActive = command.IsActive;
                site.IsPrivate = !command.IsPublic;

                uow.Context.Update(site);

                uow.Complete();
            }
        }

        private void Validate(UpdateSiteCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            long siteId = command.SiteId;            
            var siteTitle = command.SiteTitle;
            if (uow.Context.Query<Entities.Sites.Site>().Any(s => s.Title == siteTitle && s.Id != siteId))
            {
                throw new ValidationException($"Site with '{siteTitle}' title already exist");
            }

            PageName siteName = command.Name;
            if (uow.Context.Query<Entities.Sites.Site>().Any(s => s.Name == siteName && s.Id != siteId))
            {
                throw new ValidationException($"Site with '{siteName}' url already exist");
            }
        }
    }
}
