namespace Roham.Domain.Entities.Parties
{
    public class AddressMapping : IdentifiableMap<Address>
    {
        public AddressMapping()
        {
            Map(x => x.AddressType);
            Map(x => x.AddressLine1);
            Map(x => x.AddressLine2);
            Map(x => x.AddressLine3);            
            Map(x => x.PostCode);
            Map(x => x.Suburb);
            Map(x => x.State);
            Map(x => x.Country);

            References(x => x.Party).Column(MappingNames.RefId<Party>());
        }
    }
}
