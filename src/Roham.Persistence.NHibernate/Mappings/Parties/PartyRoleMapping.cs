using FluentNHibernate;

namespace Roham.Domain.Entities.Parties
{
    public class PartyRoleMapping : AggregateRootMap<PartyRole>
    {
        public PartyRoleMapping()
        {
            Map(x => x.Name);
            Map(x => x.Description);

            HasManyToMany<Party>(Reveal.Member<PartyRole>(PartyRole.NameOfParties))
                .AsSet()
                .Table(MappingNames.PartyRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<PartyRole>())
                .ChildKeyColumn(MappingNames.RefId<Party>())
                .Inverse()
                .Cascade.None();
        }
    }
}
