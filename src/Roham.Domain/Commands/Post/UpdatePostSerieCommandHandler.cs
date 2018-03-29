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
    public class UpdatePostSerieCommandHandler : AbstractCommandHandler<UpdatePostSerieCommand>
    {
        public UpdatePostSerieCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdatePostSerieCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(uow, command);

                var postSerie = uow.Context.Query<PostSerie>().SingleOrDefault(j => j.Id == command.Id);
                postSerie.Name = command.Name;
                postSerie.Title = command.Title;                
                postSerie.Description = command.Description;
                postSerie.IsPrivate = command.IsPrivate;

                uow.Context.Update(postSerie);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, UpdatePostSerieCommand command)
        {
            // TODO:
        }
    }
}