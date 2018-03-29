using System;
using Roham.Contracts.Commands.Post;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Post
{
    [AutoRegister]
    public class SavePostCommandHandler : PersistPostCommandHandler<SavePostCommand>
    {
        public SavePostCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(SavePostCommand command)
        {   
            base.OnHandle(command);
        }

        protected override bool NewRevisionRequired => false;
    }
}
