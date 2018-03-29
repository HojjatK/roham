namespace Roham.Domain.Entities.Parties
{
    public class TelephoneMapping : IdentifiableMap<Telephone>
    {
        public TelephoneMapping()
        {
            Map(x => x.Type);
            Map(x => x.Area);
            Map(x => x.Number);

            References(x => x.Party).Column(MappingNames.RefId<Party>());
        }
    }
}
