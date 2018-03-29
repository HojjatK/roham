using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class ZoneMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenZoneEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestZoneMapping()
            {
                var user1 = GetOrCreateUser("user1");
                var site1 = GetOrCreateSite("site1");
                var zoneType1 = ZoneTypeCodes.Blog;
                var zone1 = GetOrCreateZone(site1, "test.zone.mapping.1");

                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                var post2 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Save(post2);
                Session.Flush();

                new PersistenceSpecification<Zone>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, new PageName("test.zone.mapping.2"))
                    .CheckProperty(x => x.ZoneType, zoneType1)
                    .CheckProperty(x => x.Title, "test zone mapping")
                    .CheckProperty(x => x.IsActive, true)
                    .CheckProperty(x => x.IsPrivate, true)
                    .CheckProperty(x => x.Description, "test zone mapping description")
                    .CheckReference(x => x.Site, site1)                    
                    .CheckList(x => x.Entries, new List<Post> {
                        post1, post2 },
                        (z, e) => { e.Zone = z; z.Entries.Add(e); })
                    .VerifyTheMappings();
            }
        }
    }
}
