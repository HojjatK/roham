using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Commands.Role;
using Roham.Lib.Ioc;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Role
{
    [AutoRegister]
    public class AddRoleCommandHandler : AbstractCommandHandler<AddRoleCommand>
    {
        public AddRoleCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddRoleCommand command)
        {
            var roleType = RoleTypeCodes.User;
            if (!Enum.TryParse(command.RoleType, out roleType))
            {
                throw new ArgumentException($"{command.RoleType} role type is not in the range");
            }

            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var newRole = new Entities.Security.Role
                {
                    Name = command.Name,
                    Description = command.Description,
                    IsSystemRole = false,
                    RoleType = roleType,
                };
                
                // Get system role for specified role type
                var systemRole = uow.Context
                    .Query<Entities.Security.Role>()
                    .SingleOrDefault(r => r.IsSystemRole && r.RoleType == roleType);
                if (systemRole == null)
                {
                    throw new EntityNotFoundException($"No system role fond for RoleType: {roleType}");
                }
                // Copy app functions from the relating system role
                foreach (var appFunction in systemRole.GetFunctions())
                {
                    newRole.AppFunctions.Add(appFunction);
                }
                
                uow.Context.Add(newRole);

                uow.Complete();
            }
        }

        private void Validate(AddRoleCommand command, IPersistenceUnitOfWork uow)
        {
            // check duplicate name
            string newRoleName = command.Name;
            if (uow.Context.Query<Entities.Security.Role>().Any(s => s.Name == newRoleName))
            {
                throw new ValidationException($"Role with '{newRoleName}' name already exist");
            }
        }
    }
}
