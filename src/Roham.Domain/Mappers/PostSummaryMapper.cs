using System.Linq;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Posts;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PostSummaryMapper : PostItemMapper<PostSummaryDto>, IEntityMapper<PostSummaryDto, Post>
    {
        public PostSummaryDto Map(Post post)
        {
            var result = new PostSummaryDto();
            Fill(result, post);

            result.ConentSummary = post.LatestRevision.Summary;
            result.Content = GetBodyContent(post);
            result.Links = post.Links.Select(l => new LinkDto { Type = l.Type, Ref = l.Ref }).ToList();

            return result;
        }
    }
}
