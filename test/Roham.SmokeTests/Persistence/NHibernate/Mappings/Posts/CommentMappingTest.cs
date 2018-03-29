using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Entries;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class CommentMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenCommentEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestCommentMapping()
            {
                // given
                var user1 = GetOrCreateUser("user1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);

                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<Comment>(Session, new CustomEqualityComparer())
                   .CheckProperty(c => c.AuthorName, "test comment author")
                   .CheckProperty(c => c.AuthorUrl, @"http:\\test.author.com\test\url")
                   .CheckProperty(c => c.AuthorEmail, "author@emailAddress.com")
                   .CheckProperty(c => c.AuthorIp, "127.0.0.1")
                   .CheckProperty(c => c.Body, "<p>An awesome test comment</p>")
                   .CheckProperty(c => c.Posted, DateTime.Now)
                   .CheckProperty(c => c.Status, CommentStatus.NotSpam)
                   .CheckProperty(c => c.IsSpam, false)
                   .CheckProperty(c => c.RevisionNumber, 1)
                   .CheckReference(c => c.Post, post1)
                   .VerifyTheMappings();
            }

            [Test]
            public void TestSpamCommentMapping()
            {
                // given
                var user1 = GetOrCreateUser("user1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1, commentsCount: 3);

                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<Comment>(Session, new CustomEqualityComparer())
                   .CheckProperty(c => c.AuthorName, "junk comment author")
                   .CheckProperty(c => c.AuthorUrl, @"http:\\test.author.com\test\url")
                   .CheckProperty(c => c.AuthorEmail, "junk@emailAddress.com")
                   .CheckProperty(c => c.AuthorIp, "127.0.0.1")
                   .CheckProperty(c => c.Body, "<p>test spam comment</p>")
                   .CheckProperty(c => c.Posted, DateTime.Now)
                   .CheckProperty(c => c.Status, CommentStatus.Spam)
                   .CheckProperty(c => c.IsSpam, true)
                   .CheckProperty(c => c.RevisionNumber, 1)
                   .CheckReference(c => c.Post, post1)
                   .VerifyTheMappings();
            }
        }
    }
}
