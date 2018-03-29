using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Parties;

namespace Roham.Persistence.NHibernate.Mappings.Parties
{
    public class TelephoneMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenTelephoneEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestTelephoneMapping()
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
                new PersistenceSpecification<Telephone>(Session, new CustomEqualityComparer())
                    .CheckProperty(a => a.Area, "02")
                    .CheckProperty(a => a.Number, "11223344")
                    .CheckProperty(a => a.Type, TelephoneTypes.Home)
                    .CheckReference(a => a.Party, person)
                    .VerifyTheMappings();
            }
        }
    }
}
