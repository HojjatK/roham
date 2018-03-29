using System;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.User;
using Roham.Lib.Cryptography;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class ChangePasswordCommandHandler : AbstractCommandHandler<ChangePasswordCommand>
    {
        public ChangePasswordCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(ChangePasswordCommand command)
        {   
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);
                
                var newPasswordAlgorithm = (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), user.PasswordHashAlgorithm);
                user.PasswordHashAlgorithm = command.PasswordHashAlgorithm;
                user.PasswordHash = command.NewPasswordHash;
                                
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
