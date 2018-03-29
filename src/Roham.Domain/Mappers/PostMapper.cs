using System.Linq;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Posts;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class PostMapper : PostItemMapper<PostDto>, IEntityMapper<PostDto, Post>
    {
        private readonly IEntityMapper<CommentDto, Comment> _commentMapper;

        public PostMapper(IEntityMapper<CommentDto, Comment> commentMapper)
        {
            _commentMapper = commentMapper;
        }

        public PostDto Map(Post post)
        {
            var result = new PostDto();
            Fill(result, post);

            result.DisableDiscussionDays = post.DisableDiscussionDays;
            result.IsRatingEnabled = post.IsRatingEnabled;
            result.IsAnonymousCommentAllowed = post.IsAnonymousCommentAllowed;
            result.CategoriesCommaSeperated = string.Join(",", post.Categories.Select(t => t.Name).ToList());
            result.ConentSummary = post.LatestRevision.Summary;
            result.Content = GetBodyContent(post);
            result.Comments = post.Comments.ToList().Select(c => _commentMapper.Map(c)).ToList();
            result.Links = post.Links.Select(l => new LinkDto { Type = l.Type, Ref = l.Ref }).ToList();

            return result;
        }
    }
}
