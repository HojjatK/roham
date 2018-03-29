using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Parties;

namespace Roham.Persistence.NHibernate.Mappings.Parties
{
    public class OrganisationMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenOrganisationEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestOrganisationMapping()
            {   
                // assert 
                new PersistenceSpecification<Organisation>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, "Test organisation mapping")                    
                    .CheckList(x => x.PartyRoles, new List<PartyRole> {
                        new PartyRole { Name = "test party role", Description = "this is test party role desc" } },
                        (o, pr) => o.PartyRoles.Add(pr))
                    .CheckInverseList(x => x.Addresses, new List<Address> {
                        new Address { AddressType = AddressTypes.Residential, AddressLine1 = "address", Country =  "Australia"},
                        new Address { AddressType = AddressTypes.NonResidential, AddressLine1 = "non-residential address", Country =  "Australia" } },
                        (o, a) => { a.Party = o; o.Addresses.Add(a); })
                    .CheckInverseList(x => x.Telephones, new List<Telephone> {
                        new Telephone { Type = TelephoneTypes.Mobile, Number = "04400555666" } },
                        (o, t) => { t.Party = o; o.Telephones.Add(t); })
                    .VerifyTheMappings();
            }
        }
    }
}
