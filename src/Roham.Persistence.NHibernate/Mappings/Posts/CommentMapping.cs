namespace Roham.Domain.Entities.Posts
{
    public class CommentMapping : AggregateRootMap<Comment>
    {
        public CommentMapping()
        {
            Map(x => x.AuthorName);
            Map(x => x.AuthorUrl);
            Map(x => x.AuthorEmail);
            Map(x => x.AuthorIp);
            Map(x => x.Body);
            Map(x => x.Posted);
            Map(x => x.Status);
            Map(x => x.RevisionNumber);

            References(x => x.Post, MappingNames.RefId<Post>());
        }
    }
}
