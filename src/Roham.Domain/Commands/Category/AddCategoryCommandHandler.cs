/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Contracts.Commands.Category;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.Exceptions;

namespace Roham.Domain.Commands.Category
{
    [AutoRegister]
    public class AddCategoryCommandHandler : AbstractCommandHandler<AddCategoryCommand>
    {
        public AddCategoryCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(AddCategoryCommand command)
        {
            if (command == null)
            {
                throw new NullReferenceException(nameof(AddCategoryCommand));
            }

            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var siteId = command.SiteId;
                var site = uow.Context.Query<Entities.Sites.Site>().SingleOrDefault(s => s.Id == siteId);
                if (site == null)
                {
                    throw new EntityNotFoundException($"Site with Id:{siteId} not found");
                }

                Entities.Posts.Category parent = null;
                var parentId = command.ParentId;
                if (parentId != null && parentId > 0)
                {
                    parent = uow.Context.Query<Entities.Posts.Category>().Single(c => c.Id == parentId);
                    if (parent.Site.Id != siteId)
                    {
                        throw new EntityNotFoundException($"Site with Id:{siteId} is not the same of parnet site id:{parent.Site.Id}");
                    }
                }

                var newCategory = new Entities.Posts.Category
                {
                    Name = command.Name,
                    Site = site,
                    Parent = parent,
                    IsPrivate = !command.IsPublic,
                    Description = command.Description,
                };
                uow.Context.Add(newCategory);

                uow.Complete();
            }
        }

        private void Validate(AddCategoryCommand command, IPersistenceUnitOfWork uow)
        {
            var siteId = command.SiteId;
            if (siteId == 0)
            {
                throw new ValidationException("SiteId is not valid");
            }

            if (!uow.Context.Query<Entities.Sites.Site>().Any(s => s.Id == siteId))
            {
                throw new ValidationException($"Site with id:{siteId} not found");
            }

            string newCategoryName = command.Name;
            if (uow.Context.Query<Entities.Posts.Category>().Any(s => s.Name == newCategoryName))
            {
                throw new ValidationException($"Category with '{newCategoryName}' name already exist");
            }
        }
    }
}