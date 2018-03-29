namespace Roham.Domain.Entities.Parties
{
    public class OrganisationMapping : IdentifiableSubclassMap<Organisation>
    {
        public OrganisationMapping()
        {
            Map(x => x.Name);
        }
    }
}
