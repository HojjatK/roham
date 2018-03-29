using Roham.Domain.Entities.Posts;

namespace Roham.Domain.Entities.Security
{
    public class PostPermissionMapping : IdentifiableMap<PostPermission>
    {
        public PostPermissionMapping()
        {
            Map(x => x.Read);
            Map(x => x.Create);
            Map(x => x.Update);
            Map(x => x.Delete);
            Map(x => x.Execute);

            References(x => x.User).Column(MappingNames.RefId<User>());
            References(x => x.Post).Column(MappingNames.RefId<Post>());
        }
    }
}
