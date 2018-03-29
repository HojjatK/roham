using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Parties;

namespace Roham.Persistence.NHibernate.Mappings.Parties
{
    public class PartyRoleMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPartyRoleEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPartyRoleMapping()
            {
                // assert
                new PersistenceSpecification<PartyRole>(Session, new CustomEqualityComparer())
                   .CheckProperty(x => x.Name, "test party role mapping")
                   .CheckProperty(x => x.Description, "test party role mapping description")
                   .VerifyTheMappings();
            }
        }
    }
}
