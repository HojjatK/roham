using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class DeleteUserRoleCommandHandler : AbstractCommandHandler<DeleteUserRoleCommand>
    {
        public DeleteUserRoleCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteUserRoleCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);

                string roleName = command.RoleName;
                var roleToRemove = user.Roles.SingleOrDefault(r => r.Name == roleName);
                if (roleToRemove == null)
                {
                    throw new EntityNotFoundException($"User does not have role: {roleName}");
                }
                user.Roles.Remove(roleToRemove);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
