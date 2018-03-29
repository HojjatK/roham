using System;
using System.Linq;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Commands.User;
using Roham.Lib.Cryptography;
using Roham.Lib.Ioc;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class AddUserCommandHandler : AbstractCommandHandler<AddUserCommand>
    {
        public AddUserCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddUserCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                Person person = null;
                if (command.IsIndividual)
                {
                    person = new Person
                    {
                        Title = command.Title,
                        GivenName = command.GivenName ?? "",
                        Surname = command.Surname ?? ""
                    };

                    if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
                    {
                        person.Telephones.Add(new Telephone { Type = TelephoneTypes.Mobile, Number = command.PhoneNumber, Party = person });
                    }
                    uow.Context.Add(person);
                }

                var newUser = new Entities.Security.User
                {
                    UserName = command.UserName,
                    Email = command.Email,
                    SecurityStamp = Guid.NewGuid().ToString(), // initialize security stamp with a random value
                    AccessFailedCount = 0,
                    IsSystemUser = command.IsSystemUser,
                    PasswordHashAlgorithm = HashAlgorithm.PBKDF2.ToString(),
                    Status = UserStatus.Active,
                    Party = person,
                    PhoneNumber = command.PhoneNumber,
                    EmailConfirm = false,
                    PhoneNumberConfirmed = false,
                };

                foreach (var roleId in command.RoleIds)
                {
                    var role = uow.Context.FindById<Entities.Security.Role>(roleId);
                    if (role == null)
                    {
                        throw new EntityNotFoundException($"Role (id={roleId}) not found");
                    }
                    newUser.Roles.Add(role);
                }
                uow.Context.Add(newUser);
                var ctx = uow.Context as IPersistenceContextExplicit;
                if (ctx != null)
                {
                    ctx.Flush();
                }

                foreach (var siteId in command.SiteIds)
                {
                    var site = uow.Context.FindById<Entities.Sites.Site>(siteId);
                    if (site == null)
                    {
                        throw new EntityNotFoundException($"Site (id={siteId}) not found");
                    }
                    site.Users.Add(newUser);
                }

                uow.Complete();
            }
        }

        private void Validate(AddUserCommand command, IPersistenceUnitOfWork uow)
        {
            // user should have at least one role
            if (command.RoleIds == null || !command.RoleIds.Any())
            {
                throw new ValidationException("No role has been assigned to User");
            }

            // user with the same email should not exist
            if (uow.Context.Query<Entities.Security.User>().Any(s => s.Email == command.Email || s.UserName == command.Email))
            {
                throw new ValidationException($"User with same email: '{command.Email}' already exist");
            }

            // user with the same username should not exist
            if (uow.Context.Query<Entities.Security.User>().Any(s => s.UserName == command.UserName || s.Email == command.UserName))
            {
                throw new ValidationException($"User with same username: '{command.UserName}' already exist");
            }
        }
    }
}
