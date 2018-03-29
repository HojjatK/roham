/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/

using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Posts;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class CategoryMapper : IEntityMapper<CategoryDto, Category>
    {
        public CategoryDto Map(Category category)
        {
            var result = Convert(category);            
            return result;
        }

        private CategoryDto Convert(Category category)
        {
            return new CategoryDto
            {
                Uid = category.Uid.ToString(),
                Id = category.Id,
                ParentId = category.Parent?.Id,
                ParentName = category.Parent?.Name,
                SiteId = category.Site.Id,
                SiteTitle = category.Site.Title,
                Name = category.Name,
                Description = category.Description,
                IsPublic = !category.IsPrivate,
            };
        }
    }
}