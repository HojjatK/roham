using System;
using System.Linq;
using Roham.Lib.Domain.Exceptions;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class AddUserLoginCommandHandler : AbstractCommandHandler<AddUserLoginCommand>
    {
        public AddUserLoginCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddUserLoginCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);

                var loginProvider = command.LoginProvider;
                var providerKey = command.ProviderKey;
                if (user.UserLogins.Any(c => c.LoginProvider == loginProvider && c.ProviderKey == providerKey))
                {
                    throw new EntityAlreadyExistException($"LoginProvider: {loginProvider} already exist for the user");
                }
                var newUserLogin = new UserLogin
                {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey,
                    User = user,
                };
                user.UserLogins.Add(newUserLogin);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
