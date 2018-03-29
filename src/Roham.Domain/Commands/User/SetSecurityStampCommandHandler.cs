using System;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class SetSecurityStampCommandHandler : AbstractCommandHandler<SetSecurityStampCommand>
    {
        public SetSecurityStampCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(SetSecurityStampCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);

                user.SecurityStamp = command.SecurityStamp;
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
