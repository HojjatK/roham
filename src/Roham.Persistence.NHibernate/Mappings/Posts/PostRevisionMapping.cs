namespace Roham.Domain.Entities.Posts
{
    public class PostRevisionMapping : IdentifiableMap<PostRevision>
    {
        public PostRevisionMapping()
        {
            Map(x => x.RevisionNumber);
            Map(x => x.Summary);            
            Map(x => x.Author);
            Map(x => x.TagsCommaSeperated);
            Map(x => x.RevisedDate);
            Map(x => x.ReviseReason);
            Map(x => x.PublishedDate);
            Map(x => x.PublisherRoleName);
            Map(x => x.ApprovedDate);
            Map(x => x.ApproverRoleName);
            Map(x => x.BodyEncoding);
            Map(x => x.Format);
            Map(x => x.ViewsCount);
            Map(x => x.Body);
            Map(x => x.BodyImage);

            References(x => x.Post, MappingNames.RefId<Post>());
            References(x => x.Reviser, MappingNames.ReviserId);
            References(x => x.Approver, MappingNames.ApproverId);
            References(x => x.Publisher, MappingNames.PublisherId);
        }
    }
}