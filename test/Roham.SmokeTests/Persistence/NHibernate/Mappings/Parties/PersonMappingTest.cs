using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Parties;

namespace Roham.Persistence.NHibernate.Mappings.Parties
{
    public class PersonMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPersonEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPersonMapping()
            {
                // assert
                new PersistenceSpecification<Person>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Title, "Mr")
                    .CheckProperty(x => x.GivenName, "test")
                    .CheckProperty(x => x.Surname, "person mapping")
                    .CheckList(x => x.PartyRoles, new List<PartyRole> {
                        new PartyRole { Name = "test", Description = "desc" } }, (o, pr) => o.PartyRoles.Add(pr))
                    .CheckInverseList(x => x.Addresses, new List<Address> {
                        new Address { AddressType = AddressTypes.Residential, AddressLine1 = "address", Country =  "Australia" },
                        new Address { AddressType = AddressTypes.NonResidential, AddressLine1 = "non-residential address", Country =  "Australia" } },
                        (p, a) => { a.Party = p; p.Addresses.Add(a); })
                    .CheckInverseList(x => x.Telephones, new List<Telephone> {
                        new Telephone { Type = TelephoneTypes.Mobile, Number = "04400555666" } },
                        (p, t) => { t.Party = p; p.Telephones.Add(t); })
                    .VerifyTheMappings();
            }
        }
    }
}
