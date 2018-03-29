using FluentNHibernate.Mapping;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities
{
    public class IdentifiableMap<T> : ClassMap<T> where T : Identifiable
    {
        public IdentifiableMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
        }
    }

    public class IdentifiableSubclassMap<T> : SubclassMap<T> where T : Identifiable
    {
        public IdentifiableSubclassMap()
        {
            KeyColumn("Id");
        }
    }

    public class AggregateRootMap<T> : IdentifiableMap<T> where T : AggregateRoot
    {
        protected AggregateRootMap()
        {
            Map(x => x.Uid).Not.Nullable().UniqueKey("Uid_UniqueKey");
        }
    }
}
