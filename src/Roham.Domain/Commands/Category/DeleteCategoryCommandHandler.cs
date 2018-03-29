/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Linq;
using Roham.Contracts.Commands.Category;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.Persistence;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Ioc;

namespace Roham.Domain.Commands.Category
{
    [AutoRegister]
    public class DeleteCategoryCommandHandler : AbstractCommandHandler<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandHandler(Func<IPersistenceUnitOfWorkFactory> uowFactoryResolver) : base(uowFactoryResolver) { }

        protected override void OnHandle(DeleteCategoryCommand command)
        {
            using (var uow = UowFactory.Create())
            {
                var categoryToDelete = uow.Context.FindById<Entities.Posts.Category>(command.CategoryId);                
                Validate(categoryToDelete, uow);

                uow.Context.Remove(categoryToDelete);

                uow.Complete();
            }
        }

        private void Validate(Entities.Posts.Category categoryToDelete, IPersistenceUnitOfWork uow)
        {
            var categoryId = categoryToDelete.Id;

            // should check the category is used in posts or not?
            var categoryUsed = uow.Context.Query<Entities.Posts.Post>()
                                          .SelectMany(p => p.Tags)
                                          .Any(t => t.Id == categoryId);
            if (categoryUsed)
            {
                throw new ValidationException($"Category '{categoryToDelete.Name}' is used in post entries" );
            }
        }
    }
}