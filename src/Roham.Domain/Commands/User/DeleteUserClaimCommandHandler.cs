using System;
using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain.Exceptions;
using Roham.Contracts.Commands.User;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.User
{
    [AutoRegister]
    public class DeleteUserClaimCommandHandler : AbstractCommandHandler<DeleteUserClaimCommand>
    {
        public DeleteUserClaimCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteUserClaimCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var user = uow.Context.FindById<Entities.Security.User>(command.UserId);
                var claimType = command.ClaimType;
                var claim = user.UserClaims.SingleOrDefault(c => c.ClaimType == claimType);
                if (claim == null)
                {
                    throw new EntityNotFoundException($"Claim: {claimType} not found for the user");
                }
                user.UserClaims.Remove(claim);
                uow.Context.Update(user);

                uow.Complete();
            }
        }
    }
}
