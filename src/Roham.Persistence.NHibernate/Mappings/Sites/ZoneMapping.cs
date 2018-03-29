using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Domain.Entities.Sites
{
    public class ZoneMapping : AggregateRootMap<Zone>
    {
        public ZoneMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.ZoneType);
            Map(x => x.Title);
            Map(x => x.IsActive);
            Map(x => x.IsPrivate);
            Map(x => x.Description);

            References(x => x.Site).Column(MappingNames.RefId<Site>());
            //References(x => x.ZoneType).Column(MappingNames.RefId<ZoneType>());

            HasMany(x => x.Entries)
                .AsSet()
                .KeyColumn(MappingNames.RefId<Zone>())
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
