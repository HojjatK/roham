namespace Roham.Domain.Entities.Parties
{
    public class PersonMapping : IdentifiableSubclassMap<Person>
    {
        public PersonMapping()
        {
            Map(x => x.Title);
            Map(x => x.GivenName);
            Map(x => x.MiddleName);
            Map(x => x.Surname);
        }
    }
}
