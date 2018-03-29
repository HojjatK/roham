using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class PostLinkMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostLinkEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPostLinkMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.moderator1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<PostLink>(Session, new CustomEqualityComparer())
                   .CheckProperty(p => p.Type, "test/script")
                   .CheckProperty(p => p.Ref, "http://www.testdomain.com.au/test/testscript.js")                   
                   .CheckReference(p => p.Post, post1)
                   .VerifyTheMappings();
            }
        }
    }
}
