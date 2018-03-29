using System;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class SetUserEmailCommandHandler : AbstractCommandHandler<SetUserEmailCommand>
    {
        public SetUserEmailCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(SetUserEmailCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);
                user.Email = command.Email;
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
