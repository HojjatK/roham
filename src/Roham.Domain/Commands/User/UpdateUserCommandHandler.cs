using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Parties;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class UpdateUserCommandHandler : AbstractCommandHandler<UpdateUserCommand>
    {
        public UpdateUserCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateUserCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var user =uow.Context.FindById<Entities.Security.User>(command.Id);                

                if (command.IsIndividual)
                {
                    bool isNew = false;
                    if (user.Person == null)
                    {
                        user.Party = new Person();
                        isNew = true;
                    }

                    user.Person.Title = command.Title;
                    user.Person.GivenName = command.GivenName;
                    user.Person.MiddleName = command.MiddleName;
                    user.Person.Surname = command.Surname;

                    // phone number
                    if (!user.PhoneNumberConfirmed)
                    {
                        user.PhoneNumber = command.PhoneNumber;
                        var mobile = user.Person.Telephones.FirstOrDefault(t => t.Type == TelephoneTypes.Mobile);
                        if (mobile == null)
                        {
                            mobile = new Telephone { Type = TelephoneTypes.Mobile, Number = command.PhoneNumber, Party = user.Person };
                            user.Person.Telephones.Add(mobile);                            
                        }
                        mobile.Number = command.PhoneNumber;
                    }
                    else
                    {
                        if (!string.Equals(user.PhoneNumber, command.PhoneNumber))
                        {
                            throw new ValidationException("User phone number already confirmed and cannot be changed.");
                        }
                    }

                    if (isNew)
                    {
                        uow.Context.Add(user.Party);
                    }

                    // role ids
                    var existingRoles = user.Roles.ToDictionary(r => r.Id);
                    foreach(var roleId in command.RoleIds)
                    {
                        if (!existingRoles.Keys.Contains(roleId))
                        {
                            var role = uow.Context.FindById<Entities.Security.Role>(roleId);
                            if (role == null)
                            {
                                throw new EntityNotFoundException($"Role (id={roleId}) not found");
                            }
                            user.Roles.Add(role);
                        }
                    }
                    // admin role cannot be de-assinged
                    var deletingRoles = user.Roles.Where(r => r.RoleType != Entities.Security.RoleTypeCodes.SystemAdmin && !command.RoleIds.Contains(r.Id)).ToList();
                    foreach(var roleToDelete in deletingRoles)
                    {
                        user.Roles.Remove(roleToDelete);
                    }

                    // site ids
                    var existingSites = user.GetUserSites().ToDictionary(s => s.Id);
                    foreach (var siteId in command.SiteIds)
                    {
                        if (!existingSites.Keys.Contains(siteId))
                        {
                            var site = uow.Context.FindById<Entities.Sites.Site>(siteId);
                            if (site == null)
                            {
                                throw new EntityNotFoundException($"Site (id={siteId}) not found");
                            }
                            site.Users.Add(user);
                        }
                    }

                    var deletingSites = user.GetUserSites().Where(s => !command.SiteIds.Contains(s.Id));
                    foreach (var siteToDelete in deletingSites)
                    {
                        siteToDelete.Users.Remove(user);
                    }
                }

                uow.Context.Update(user);
                uow.Complete();
            }
        }

        private void Validate(UpdateUserCommand command, IPersistenceUnitOfWork uow)
        {
            // user should have at least one role
            if (command.RoleIds == null || !command.RoleIds.Any())
            {
                throw new ValidationException("No role has been assigned to User");
            }
        }
    }
}
