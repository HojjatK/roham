using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Role;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Role
{
    [AutoRegister]
    public class DeleteRoleCommandHandler : AbstractCommandHandler<DeleteRoleCommand>
    {
        public DeleteRoleCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteRoleCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var roleToDelete = uow.Context
                    .FindById<Entities.Security.Role>(command.Id);

                Validate(uow, roleToDelete);
                uow.Context.Remove(roleToDelete);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, Entities.Security.Role roleToDelete)
        {
            if (roleToDelete.IsSystemRole)
            {
                throw new ValidationException("A system role cannot be deleted.");
            }
            long roleId = roleToDelete.Id;
            bool hasUsers = uow.Context
                .Query<Entities.Security.User>()
                .Any(u => u.Roles.Any(r => r.Id == roleId));

            if (hasUsers)
            {
                throw new ValidationException("Role is assigned to users and cannot be deleted.");
            }
        }
    }
}
