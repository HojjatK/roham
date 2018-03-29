using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Security
{
    public class PostWorkflowRuleMapping : AggregateRootMap<PostWorkflowRule>
    {
        public PostWorkflowRuleMapping()
        {
            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.ReturnToAuthorForPublish);

            References(x => x.ApproverRole).Column(MappingNames.ApproverRoleId);
            References(x => x.PublisherRole).Column(MappingNames.PublisherRoleId);
            References(x => x.Site).Column(MappingNames.RefId<Site>());
        }
    }
}
