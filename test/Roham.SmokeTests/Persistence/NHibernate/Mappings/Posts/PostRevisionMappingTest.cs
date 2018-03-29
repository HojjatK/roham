using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class PostRevisionMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostRevision : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPostRevisionMapping()
            {
                var user1 = GetOrCreateUser("test.moderator1");
                var user2 = GetOrCreateUser("test.approver");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog zone");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1, 20, 5, 2);

                Session.Save(post1);
                Session.Flush();

                // assert
                new PersistenceSpecification<PostRevision>(Session, new CustomEqualityComparer())
                    .CheckProperty(e => e.RevisionNumber, 1)
                    .CheckProperty(e => e.Summary, "test summary")                    
                    .CheckProperty(e => e.Author, "test")
                    .CheckProperty(e => e.TagsCommaSeperated, "tag1, tag2")
                    .CheckProperty(e => e.RevisedDate, DateTime.Now.AddDays(-2))
                    .CheckProperty(e => e.ReviseReason, "test reason")
                    .CheckProperty(e => e.PublishedDate, DateTime.Now.AddDays(-1))
                    .CheckProperty(e => e.PublisherRoleName, "moderator")
                    .CheckProperty(e => e.ApprovedDate, DateTime.Now)
                    .CheckProperty(e => e.ApproverRoleName, "approver")
                    .CheckProperty(e => e.Format, ContentFormats.Html)
                    .CheckProperty(e => e.ViewsCount, (long)2)
                    .CheckProperty(e => e.Body, "<div><p>test body for <span>an awesome test blog</span></p></div>")
                    .CheckReference(e => e.Post, post1)
                    .CheckReference(e => e.Reviser, user1)
                    .CheckReference(e => e.Publisher, user1)
                    .CheckReference(e => e.Approver, user2)
                    .VerifyTheMappings();
            }
        }
    }
}
