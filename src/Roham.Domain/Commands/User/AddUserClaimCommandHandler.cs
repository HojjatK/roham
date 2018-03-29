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
    public class AddUserClaimCommandHandler : AbstractCommandHandler<AddUserClaimCommand>
    {
        public AddUserClaimCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddUserClaimCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context
                    .FindById<Entities.Security.User>(command.UserId);

                var claimType = command.ClaimType;
                if (user.UserClaims.Any(c => c.ClaimType == claimType))
                {
                    throw new EntityAlreadyExistException($"Claim: {claimType} already exist for the user");
                }
                var newClaim = new UserClaim
                {
                    ClaimType = claimType,
                    ClaimValue = command.ClaimValue,
                    User = user
                };
                user.UserClaims.Add(newClaim);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
