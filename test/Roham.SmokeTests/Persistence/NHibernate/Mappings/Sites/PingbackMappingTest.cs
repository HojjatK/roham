using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Sites;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class PingbackMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPingbackEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPingbackMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.moderator1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<Pingback>(Session, new CustomEqualityComparer())
                   .CheckProperty(p => p.TargetUri, @"https//pingback.target.uri/test")
                   .CheckProperty(p => p.TargetTitle, "Pingback target title")
                   .CheckProperty(p => p.IsSpam, true)
                   .CheckProperty(p => p.IsTrackback, true)
                   .CheckProperty(p => p.Received, DateTime.Now)
                   .CheckReference(p => p.Post, post1)
                   .VerifyTheMappings();
            }
        }
    }
}
