/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System.Linq;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindPostByNameQueryHandler : AbstractQueryHandler<FindPostByNameQuery, PostDto>
    {
        public FindPostByNameQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override PostDto OnHandle(FindPostByNameQuery query)
        {
            var siteName = query.SiteName;
            var zoneName = query.ZoneName;
            var postName = query.PostName;

            PostDto postDto = null;
            using (var uow = _uowFactory.CreateReadOnly())
            {
                long? siteId, zoneId;
                siteId = uow.Context.Query<Site>()
                    .Where(s => s.Name == siteName)
                    .Select(s => s.Id).FirstOrDefault();

                if (siteId.HasValue)
                {
                    var sId = siteId.Value;
                    zoneId = uow.Context.Query<Zone>()
                        .Where(z => z.Site.Id == sId && z.Name == zoneName)
                        .Select(z => z.Id).FirstOrDefault();
                    if (zoneId.HasValue)
                    {
                        var zId = zoneId.Value;
                        var post = uow.Context.Query<Entities.Posts.Post>()
                            .Where(p => p.Site.Id == sId && p.Zone.Id == zId && p.Name == postName)
                            .FirstOrDefault();

                        if (post != null)
                        {
                            postDto = _entityMapperFactory.Create<PostDto, Entities.Posts.Post>().Map(post);
                        }
                    }
                }
                uow.Complete();
            }

            return postDto;
        }
    }
}