using System;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class SetUserEmailConfirmCommandHandler : AbstractCommandHandler<SetUserEmailConfirmCommand>
    {
        public SetUserEmailConfirmCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(SetUserEmailConfirmCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);
                user.EmailConfirm = command.Confirmed;
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
