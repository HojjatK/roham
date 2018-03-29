/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Ioc;
using Roham.Lib.Domain;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Posts;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PostSerieMapper : IEntityMapper<PostSerieDto, PostSerie>
    {
        public PostSerieDto Map(PostSerie serie)
        {
            return new PostSerieDto
            {
                Uid = serie.Uid.ToString(),
                Id = serie.Id,
                SiteId = serie.Site?.Id,
                SiteTitle = serie.Site?.Title,
                Name = serie.Name,
                Title = serie.Title,
                Description = serie.Description,
                IsPrivate = serie.IsPrivate,
            };
        }
    }
}