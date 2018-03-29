using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Domain.Entities.Jobs;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class DeleteUserCommandHandler : AbstractCommandHandler<DeleteUserCommand>
    {
        public DeleteUserCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteUserCommand command)
        {
            using (var uow = UowFactory.Create())
            {   
                var user = uow.Context.FindById<Entities.Security.User>(command.Id);
                Validate(uow, user);
                
                foreach (var siteToDelete in user.GetUserSites())
                {
                    siteToDelete.Users.Remove(user);
                }

                uow.Context.Remove(user);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, Entities.Security.User userToDelete)
        {
            // user has assigned to site
            var userId = userToDelete.Id;
            var site = uow.Context.Query<Entities.Sites.Site>().FirstOrDefault(s => s.Users.Any(u => u.Id == userId));
            if (site != null)
            {
                throw new ValidationException($"User is assigned to site: {site.Name}");
            }

            // user is creator of post
            if (uow.Context.Query<Entities.Posts.Post>().Any(p => p.Creator.Id == userId))
            {
                throw new ValidationException("User has associated post entry, please delete all user's posts first and try again");
            }

            // user is owner of job
            if (uow.Context.Query<Entities.Jobs.Job>().Any(j => j.Owner.Id == userId))
            {
                throw new ValidationException($"User has jobs, please delete all user's jobs first and try again.");
            }
        }
    }
}
