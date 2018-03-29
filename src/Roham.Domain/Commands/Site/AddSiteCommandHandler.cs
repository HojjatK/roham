using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Commands.Site;
using Roham.Lib.Ioc;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Site
{
    [AutoRegister]
    public class AddSiteCommandHandler : AbstractCommandHandler<AddSiteCommand>
    {
        public AddSiteCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddSiteCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var portal = uow.Context
                    .Where<Entities.Sites.Portal>(p => true)
                    .Single();

                var sysAdminRole = uow.Context
                    .Where<Entities.Security.Role>(r => r.RoleType == RoleTypeCodes.Administrator && r.IsSystemRole)
                    .Single();

                string ownerUsername = command.OwnerUsername;
                var ownerUser = uow.Context
                    .Where<Entities.Security.User>(u => u.UserName == ownerUsername)
                    .Single();

                var newSite = new Entities.Sites.Site
                {
                    Name = command.Name,
                    Title = command.SiteTitle,
                    Description = command.Description,
                    Portal = portal,
                    IsDefault = command.IsDefault,
                    IsActive = command.IsActive,
                    IsPrivate = !command.IsPublic,
                    Owner = ownerUser,
                };

                if (!ownerUser.Roles.Any(r => r.RoleType == RoleTypeCodes.Administrator))
                {
                    var siteAdminRole = uow.Context
                        .Query<Entities.Security.Role>()
                        .Where(r => r.RoleType == RoleTypeCodes.Administrator && r.IsSystemRole)
                        .SingleOrDefault();
                    ownerUser.Roles.Add(siteAdminRole);
                }

                newSite.Users.Add(ownerUser);
                uow.Context.Add(newSite);

                portal.Sites.Add(newSite);

                uow.Complete();
            }
        }

        private void Validate(AddSiteCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            var newSiteTitle = command.SiteTitle;
            if (uow.Context.Query<Entities.Sites.Site>().Any(s => s.Title == newSiteTitle))
            {
                throw new ValidationException($"Site with '{newSiteTitle}' title already exist");
            }

            PageName newSiteUrl = command.Name;
            if (uow.Context.Query<Entities.Sites.Site>().Any(s => s.Name == newSiteUrl))
            {
                throw new ValidationException($"Site with '{newSiteUrl}' url already exist");
            }
        }
    }
}
