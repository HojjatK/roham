using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Site;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Site
{
    [AutoRegister]
    public class DeleteSiteCommandHandler : AbstractCommandHandler<DeleteSiteCommand>
    {
        public DeleteSiteCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteSiteCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var siteToDelete = uow.Context
                    .FindById<Entities.Sites.Site>(command.Id);

                Validate(uow, siteToDelete);
                uow.Context.Remove(siteToDelete);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, Entities.Sites.Site siteToDelete)
        {
            long siteId = siteToDelete.Id;
            bool hasZones = uow.Context
                .Query<Entities.Sites.Zone>()
                .Any(s => s.Site.Id == siteId);

            if (hasZones)
            {
                throw new ValidationException("Site has zones, please delete them first and try again.");
            }

            bool hasPosts = uow.Context
                .Query<Entities.Posts.Post>()
                .Any(e => e.Site.Id == siteId);

            if (hasPosts)
            {
                throw new ValidationException("Site has posts, please delete them first and try again.");
            }
        }
    }
}
