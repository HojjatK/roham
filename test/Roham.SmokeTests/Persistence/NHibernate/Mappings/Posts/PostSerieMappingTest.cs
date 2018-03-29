using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class PostSerieMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostSerie : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPostSerieMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.moderator1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                var post2 = TestDataBuilder.NewPost(site1, zone1, user1);
                var post3 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Save(post2);
                Session.Save(post3);
                Session.Flush();

                // assert
                new PersistenceSpecification<PostSerie>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, new PageName("test blog serie"))
                    .CheckProperty(x => x.Title, "test blog serie")
                    .CheckProperty(x => x.Description, "this is a test blog serie")
                    .CheckProperty(x => x.IsPrivate, false)
                    .CheckReference(x => x.Site, site1)
                    .CheckList(x => x.Posts, new List<Post> { post1, post2, post3 }, (s, e) => { e.Serie = s; s.Posts.Add(e); })
                    .VerifyTheMappings();
            }
        }
    }
}
