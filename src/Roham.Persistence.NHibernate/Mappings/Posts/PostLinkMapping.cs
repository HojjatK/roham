using Roham.Domain.Entities;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class PostLinkMapping : IdentifiableMap<PostLink>
    {
        public PostLinkMapping()
        {
            Map(x => x.Type);
            Map(x => x.Ref);            

            References(x => x.Post, MappingNames.RefId<Post>());
        }
    }
}
