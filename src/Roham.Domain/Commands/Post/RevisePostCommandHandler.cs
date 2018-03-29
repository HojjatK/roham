using System;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Post;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Post
{
    [AutoRegister]
    public class RevisePostCommandHandler : PersistPostCommandHandler<RevisePostCommand>
    {
        public RevisePostCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(RevisePostCommand command)
        {
            base.OnHandle(command);
        }

        protected override bool NewRevisionRequired => true;
    }
}
