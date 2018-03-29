using Roham.Domain.Entities.Parties;
using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Domain.Entities.Sites
{
    public class PortalMapping : AggregateRootMap<Portal>
    {
        public PortalMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>(); ;
            Map(x => x.Title);
            Map(x => x.Description);

            References(x => x.Organisation).Column(MappingNames.RefId<Organisation>()).Nullable();
            References(x => x.Owner).Column(MappingNames.OwnerId);

            HasMany(x => x.Sites)
                .AsSet()
                .KeyColumn(MappingNames.RefId<Portal>())
                .Inverse()
                .Cascade.All();
        }
    }
}