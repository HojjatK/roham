using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Domain.Entities.Sites
{
    public class SettingMapping : AggregateRootMap<Setting>
    {
        public SettingMapping()
        {
            Map(x => x.Section).CustomType<PageNameUserType>();
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.Value);

            References(x => x.Site).Column(MappingNames.RefId<Site>());
        }
    }
}
