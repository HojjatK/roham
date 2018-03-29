using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class RatingMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenRatingEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestRatingMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.moderator1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<Rating>(Session, new CustomEqualityComparer())
                   .CheckProperty(p => p.Rate, (decimal)2.3)
                   .CheckProperty(p => p.RatedDate, DateTime.Now)
                   .CheckProperty(p => p.UserIdentity, "pingback.user")
                   .CheckProperty(p => p.UserEmail, "pingback.user@emial.com")
                   .CheckReference(p => p.Post, post1)
                   .VerifyTheMappings();
            }
        }
    }
}