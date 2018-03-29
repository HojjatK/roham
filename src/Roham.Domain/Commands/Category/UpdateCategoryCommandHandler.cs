/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/

using System;
using System.Linq;
using Roham.Contracts.Commands.Category;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.Exceptions;

namespace Roham.Domain.Commands.Category
{
    [AutoRegister]
    public class UpdateCategoryCommandHandler : AbstractCommandHandler<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(UpdateCategoryCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                Validate(command, uow);

                var categoryToUpdate = uow.Context.Query<Entities.Posts.Category>().FirstOrDefault(c => c.Id == command.CategoryId);
                if (categoryToUpdate == null)
                {
                    throw new EntityNotFoundException($"Category with id:{command.CategoryId} not found.");
                }                

                long? existingParentId = categoryToUpdate.Parent?.Id;
                var newParentId = command.ParentCategoryId;

                if (newParentId != existingParentId)
                {
                    Entities.Posts.Category parent = null;
                    if (newParentId != null && newParentId.Value > 0)
                    {
                        parent = uow.Context.Query<Entities.Posts.Category>().SingleOrDefault(c => c.Id == newParentId.Value);
                        if (parent == null)
                        {
                            throw new EntityNotFoundException($"Parent category with id:{newParentId} not found.");
                        }
                        if (parent.Site.Id != categoryToUpdate.Site.Id)
                        {
                            throw new EntityNotFoundException($"Category site Id:{categoryToUpdate.Site.Id} is not the same of parnet site id:{parent.Site.Id}");
                        }
                    }
                    categoryToUpdate.Parent = parent;
                }
                categoryToUpdate.Description = command.Description;
                categoryToUpdate.IsPrivate = !command.IsPublic;

                uow.Context.Update(categoryToUpdate);
                uow.Complete();
            }  
        }

        private void Validate(UpdateCategoryCommand command, IPersistenceUnitOfWork uow)
        {
            // empty
        }
    }
}