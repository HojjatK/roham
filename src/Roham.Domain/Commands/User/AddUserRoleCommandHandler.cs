using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class AddUserRoleCommandHandler : AbstractCommandHandler<AddUserRoleCommand>
    {
        public AddUserRoleCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddUserRoleCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);

                string roleName = command.RoleName;
                if (user.Roles.Any(r => r.Name == roleName))
                {
                    throw new EntityAlreadyExistException($"User already has {roleName} role");
                }
                var role = uow.Context
                    .Query<Entities.Security.Role>()
                    .SingleOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    throw new EntityNotFoundException($"Role: {roleName} not found");
                }
                user.Roles.Add(role);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
