using Roham.Domain.Entities.Sites;
using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Domain.Entities.Posts
{
    public class PostSeriesMapping : AggregateRootMap<PostSerie>
    {
        public PostSeriesMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.IsPrivate);

            References(x => x.Site).Column(MappingNames.RefId<Site>());

            HasMany(x => x.Posts)
                .AsSet()
                .Table(nameof(Post))
                .KeyColumn(MappingNames.RefId<PostSerie>())
                .Inverse()
                .Cascade.All();
        }
    }
}
