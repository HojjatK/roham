using System;
using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class PortalMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPortalEntity : NHibernateEntityMappingFixture
        {
            // This fixture creates a new portal entity which causes issue if already one inserted
            // as a result use a new db
            public GivenPortalEntity() : base(false) { }

            [Test]
            public void TestPortalMapping()
            {
                var admin = GetOrCreateUser("admin");

                new PersistenceSpecification<Portal>(Session, new CustomEqualityComparer())
                    .CheckProperty(p => p.Uid, Guid.NewGuid())
                    .CheckProperty(p => p.Name, new PageName("testportal-portal1"))
                    .CheckProperty(p => p.Title, "test poratl 1")
                    .CheckProperty(p => p.Owner, admin)
                    .CheckInverseList(p => p.Sites, new List<Site> {
                        new Site { Name = new PageName("testportal.site1"), Title = "Site 1", IsActive = true, IsDefault = true, Owner = admin },
                        new Site { Name = new PageName("testportal.site2"), Title = "Site 2", IsActive = false, Owner = admin } },
                        (p, s) => { s.Portal = p; p.Sites.Add(s); })
                    .VerifyTheMappings();
            }
        }
    }
}
