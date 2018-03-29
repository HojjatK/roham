/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using Roham.Contracts.Commands.Post;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Posts;

namespace Roham.Domain.Commands.Post
{   
    [AutoRegister]
    public class DeletePostSerieCommandHandler : AbstractCommandHandler<DeletePostSerieCommand>
    {
        public DeletePostSerieCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeletePostSerieCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(uow, command);
                var postSerie = uow.Context.Query<PostSerie>().SingleOrDefault(j => j.Id == command.Id);
                uow.Context.Remove(postSerie);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, DeletePostSerieCommand command)
        {
            // TODO:
        }
    }
}