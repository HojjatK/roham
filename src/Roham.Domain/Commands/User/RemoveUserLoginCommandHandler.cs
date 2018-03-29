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
    public class RemoveUserLoginCommandHandler : AbstractCommandHandler<RemoveUserLoginCommand>
    {
        public RemoveUserLoginCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(RemoveUserLoginCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);

                var loginProvider = command.LoginProvider;
                var providerKey = command.ProviderKey;

                var userLogin = user.UserLogins.SingleOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
                if (userLogin == null)
                {
                    throw new EntityNotFoundException($"Login Provider: {loginProvider} not found");
                }
                user.UserLogins.Remove(userLogin);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
