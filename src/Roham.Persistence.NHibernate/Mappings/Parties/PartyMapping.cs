using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Parties
{
    public class PartyMapping : AggregateRootMap<Party>
    {
        public PartyMapping()
        {
            HasManyToMany(x => x.PartyRoles)
                .AsSet()
                .Table(MappingNames.PartyRoleMapTableName)
                .ParentKeyColumn(MappingNames.RefId<Party>())
                .ChildKeyColumn(MappingNames.RefId<PartyRole>())
                .Cascade.None();

            HasMany(x => x.Addresses)
                .AsSet()
                .Table(nameof(Address))
                .KeyColumn(MappingNames.RefId<Party>())
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Telephones)
                .AsSet()
                .Table(nameof(Telephone))
                .KeyColumn(MappingNames.RefId<Party>())
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Users)
                .AsSet()
                .Table(nameof(User))
                .KeyColumn(MappingNames.RefId<Party>())
                .Inverse()
                .Cascade.SaveUpdate();
        }
    }
}
