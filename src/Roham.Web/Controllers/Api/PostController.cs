using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Commands.Post;
using Roham.Domain.Entities.Posts;

namespace Roham.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/post")]
    public class PostController : ApiControllerBase
    {
        public PostController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher) { }

        [HttpGet]
        [Route("")]
        public List<PostItemDto> GetPostSummaries()
        {
            return QueryExecutor.Execute(new FindAllQuery<PostItemDto, Post>());
        }

        [HttpGet]
        [Route("{id:long}")]
        public PostDto GetPost(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<PostDto, Post>(id));
        }

        [HttpPost]
        [Route("")]
        public ResultDto NewPost(PostDto postDto)
        {
            return Result(() => {
                var currentUserName = User.Identity.GetUserName();
                var command = new NewPostCommand
                {
                    SiteId = postDto.SiteId,
                    ZoneId = postDto.ZoneId,
                    SerieId = postDto.SerieId,
                    Name = postDto.Name,
                    Title = postDto.Title,
                    MetaTitle = postDto.MetaTitle,
                    MetaDescription = postDto.MetaDescription,
                    PageTemplate = postDto.PageTemplate,
                    IsPrivate = postDto.IsPrivate,
                    IsDiscussionEnabled = postDto.IsDiscussionEnabled,
                    DisableDiscussionDays = postDto.DisableDiscussionDays,
                    IsAnonymousCommentAllowed = postDto.IsAnonymousCommentAllowed,
                    IsRatingEnabled = postDto.IsRatingEnabled,
                    UserName = currentUserName,
                    TagsCommaSeparated = postDto.TagsCommaSeparated,
                    ContentSummary = postDto.ConentSummary,
                    Content = postDto.Content,
                    Links = (postDto.Links ?? new List<LinkDto>()).Select(l => new KeyValuePair<string, string>(l.Type, l.Ref)).ToList(),
                };
                CommandDispatcher.Send(command);
            });
            
        }

        [HttpPut]
        [Route("{id:long}")]
        public ResultDto RevisePost(long id, PostDto postDto)
        {
            return Result(() =>
            {
                var currentUserName = User.Identity.GetUserName();
                var command = new RevisePostCommand
                {
                    SiteId = postDto.SiteId,
                    ZoneId = postDto.ZoneId,
                    PostId = id,
                    Name = postDto.Name,
                    Title = postDto.Title,
                    MetaTitle = postDto.MetaTitle,
                    MetaDescription = postDto.MetaDescription,
                    PageTemplate = postDto.PageTemplate,
                    IsPrivate = postDto.IsPrivate,
                    IsDiscussionEnabled = postDto.IsDiscussionEnabled,
                    DisableDiscussionDays = postDto.DisableDiscussionDays,
                    IsAnonymousCommentAllowed = postDto.IsAnonymousCommentAllowed,
                    IsRatingEnabled = postDto.IsRatingEnabled,
                    UserName = currentUserName,
                    TagsCommaSeparated = postDto.TagsCommaSeparated,
                    ContentSummary = postDto.ConentSummary,
                    Content = postDto.Content,
                    Links = (postDto.Links ?? new List<LinkDto>()).Select(l => new KeyValuePair<string, string>(l.Type, l.Ref)).ToList(),
                };
                CommandDispatcher.Send(command);
            });
            
        }

        [HttpDelete]
        [Route("{id:long}")]
        public ResultDto DeletePost(long id)
        {
            return Result(() =>
            {
                var command = new DeletePostCommand
                {
                    PostId = id,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpGet]
        [Route("comment")]
        public List<CommentDto> GetComments()
        {
            return QueryExecutor.Execute(new FindAllQuery<CommentDto, Comment>());
        }

        [HttpPut]
        [Route("{postId:long}/comment/{commentId:long}")]
        public ResultDto UpdateComment(long postId, long commentId, CommentDto commentDto)
        {
            return Result(() =>
            {
                var command = new UpdateCommentCommand
                {
                    PostId = postId,
                    CommentId = commentId,                    
                    Content = commentDto.Body,
                    Status = commentDto.Status,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Route("{postId:long}/comment/{commentId:long}")]
        public ResultDto DeleteComment(long postId, long commentId)
        {
            return Result(() =>
            {
                var command = new DeleteCommentCommand
                {
                    PostId = postId,
                    CommentId = commentId,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpGet]
        [Route("serie")]
        public List<PostSerieDto> GetSeries()
        {
            return QueryExecutor.Execute(new FindAllQuery<PostSerieDto, PostSerie>());
        }

        [HttpPost]
        [Route("serie")]
        public ResultDto NewSerie(PostSerieDto serieDto)
        {
            return Result(() =>
            {
                var command = new AddPostSerieCommand
                {
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]
        [Route("serie/{id:long}")]
        public ResultDto UpdateSerie(long serieId, PostSerieDto serieDto)
        {
            return Result(() =>
            {
                var command = new UpdatePostSerieCommand
                {   
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]
        [Route("serie/{id:long}")]
        public ResultDto DeleteSerie(long serieId)
        {
            return Result(() =>
            {
                var command = new DeletePostSerieCommand
                {
                };
                CommandDispatcher.Send(command);
            });
        }
    }
}
