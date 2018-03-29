using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Posts;
using Roham.Contracts.Dtos;


namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class CommentMapper : IEntityMapper<CommentDto, Comment>
    {
        public CommentDto Map(Comment comment)
        {
            return new CommentDto
            {
                Uid = comment.Uid.ToString(),
                Id = comment.Id,
                postId = comment.Post.Id,
                postTitle = comment.Post.Title,
                AuthorName = comment.AuthorName,
                AuthorUrl = comment.AuthorUrl,
                AuthorEmail = comment.AuthorEmail,
                AuthorIp = comment.AuthorIp,
                Body = comment.Body,
                Posted = comment.Posted,
                Status = comment.Status.ToString(),
            };
        }
    }
}
