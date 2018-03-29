using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Parties;

namespace Roham.Persistence.NHibernate.Mappings.Parties
{
    public class AddressMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenAddressEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestAddressMapping()
            {
                // given
                var person = new Person
                {
                    Title = "Mr",
                    GivenName = "test.address.given.name1",
                    Surname = "test.address.surname1"
                };
                Session.Save(person);
                Session.Flush();

                // assert
                new PersistenceSpecification<Address>(Session, new CustomEqualityComparer())
                    .CheckProperty(a => a.AddressLine1, "line 1")
                    .CheckProperty(a => a.AddressLine2, "line 2")
                    .CheckProperty(a => a.AddressLine3, "line 3")
                    .CheckProperty(a => a.AddressType, AddressTypes.Residential)
                    .CheckProperty(a => a.Suburb, "test suburb")
                    .CheckProperty(a => a.State, "test state")
                    .CheckProperty(a => a.Country, "test country")
                    .CheckProperty(a => a.PostCode, "112233")
                    .CheckReference(a => a.Party, person)
                    .VerifyTheMappings();
            }
        }
    }
}
