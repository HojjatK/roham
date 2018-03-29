using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Snippets;

namespace Roham.Domain.Entities.Sites
{
    public class PingbackMapping : AggregateRootMap<Pingback>
    {
        public PingbackMapping()
        {
            Map(x => x.TargetUri);
            Map(x => x.TargetTitle);
            Map(x => x.IsSpam);
            Map(x => x.IsTrackback);
            Map(x => x.Received);

            References(x => x.Post, MappingNames.RefId<Post>());
            References(x => x.Snippet, MappingNames.RefId<Snippet>());
        }
    }
}
