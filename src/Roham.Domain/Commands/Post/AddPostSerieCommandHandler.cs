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
    public class AddPostSerieCommandHandler : AbstractCommandHandler<AddPostSerieCommand>
    {
        public AddPostSerieCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddPostSerieCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(uow, command);

                var siteId = command.SiteId;
                var site = uow.Context.Query<Entities.Sites.Site>().SingleOrDefault(s => s.Id == siteId);

                var postSerie = new PostSerie
                {
                    Site = site,
                    Name = command.Name,
                    Title = command.Title,
                    Description = command.Description,
                    IsPrivate = command.IsPrivate
                };
                uow.Context.Add(postSerie);

                uow.Complete();
            }
        }

        private void Validate(IPersistenceUnitOfWork uow, AddPostSerieCommand command)
        {
            // TODO:
        }
    }
}