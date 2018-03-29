using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Role;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.Role
{
    [AutoRegister]
    public class UpdateRoleCommandHandler : AbstractCommandHandler<UpdateRoleCommand>
    {
        public UpdateRoleCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateRoleCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var role = uow.Context
                    .FindById<Entities.Security.Role>(command.Id);

                Validate(uow, role, command);
                role.Name = command.Name;
                role.Description = command.Description;

                if (command.IncludeFuctions)
                {
                    var toAdd = new List<AppFunction>();
                    var toRemove = new List<AppFunction>();

                    var allAppFunctions = uow.Context.Query<AppFunction>().ToList().ToDictionary(ap => ap.Id);
                    var roleFunctions = role.AppFunctions.ToDictionary(f => f.Id);
                    if (command.Functions != null)
                    {
                        foreach (var appFuncDto in command.Functions)
                        {
                            if (roleFunctions.ContainsKey(appFuncDto.Id) && !appFuncDto.IsAllowed)
                            {
                                toRemove.Add(roleFunctions[appFuncDto.Id]);
                            }
                            else if (!roleFunctions.ContainsKey(appFuncDto.Id) && appFuncDto.IsAllowed)
                            {
                                toAdd.Add(allAppFunctions[appFuncDto.Id]);
                            }
                        }
                    }

                    toRemove.ForEach(f => role.AppFunctions.Remove(f));
                    toAdd.ForEach(f => role.AppFunctions.Add(f));
                }
                uow.Context.Update(role);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, Entities.Security.Role roleToUpdate, UpdateRoleCommand command)
        {
            if (roleToUpdate.IsSystemRole && command.IncludeFuctions)
            {
                if (!string.Equals((roleToUpdate.Name ?? "").Trim(), (command.Name ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationException("A system role permissions cannot be changed.");
                }
            }
        }
    }
}
