using System;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Commands.Post;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Post
{
    [AutoRegister]
    public class DeletePostCommandHandler : AbstractCommandHandler<DeletePostCommand>
    {
        public DeletePostCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeletePostCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                // TODO: validation
                var post = uow.Context.FindById<Entities.Posts.Post>(command.PostId);
                uow.Context.Remove(post);

                uow.Complete();
            }
        }
    }
}
