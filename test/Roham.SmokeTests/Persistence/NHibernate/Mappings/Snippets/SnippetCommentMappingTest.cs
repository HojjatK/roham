using System;
using NUnit.Framework;
using FluentNHibernate.Testing;
using Roham.Domain.Entities.Entries;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetCommentMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenSnippetCommentEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSnippetCommentMapping()
            {
                var user1 = GetOrCreateUser("user1");
                // given
                var snippet1 = TestDataBuilder.NewSnippet(user1);

                Session.Save(snippet1);
                Session.Flush();

                // assert
                new PersistenceSpecification<SnippetComment>(Session, new CustomEqualityComparer())
                   .CheckProperty(c => c.AuthorName, "test comment author")
                   .CheckProperty(c => c.AuthorUrl, @"http:\\test.author.com\test\url")
                   .CheckProperty(c => c.AuthorEmail, "author@emailAddress.com")
                   .CheckProperty(c => c.AuthorIp, "127.0.0.1")
                   .CheckProperty(c => c.Body, "<p>An awesome test comment</p>")
                   .CheckProperty(c => c.Posted, DateTime.Now)
                   .CheckProperty(c => c.Status, CommentStatus.NotSpam)
                   .CheckProperty(c => c.IsSpam, false)
                   .CheckProperty(c => c.RevisionNumber, 1)
                   .CheckReference(c => c.Snippet, snippet1)
                   .VerifyTheMappings();
            }

            [Test]
            public void TestSpamSnippetCommentMapping()
            {
                // given                
                var user1 = GetOrCreateUser("user1");
                var snippet1 = TestDataBuilder.NewSnippet(user1, commentsCount: 3);

                Session.Save(snippet1);
                Session.Flush();

                // assert
                new PersistenceSpecification<SnippetComment>(Session, new CustomEqualityComparer())
                   .CheckProperty(c => c.AuthorName, "junk comment author")
                   .CheckProperty(c => c.AuthorUrl, @"http:\\test.author.com\test\url")
                   .CheckProperty(c => c.AuthorEmail, "junk@emailAddress.com")
                   .CheckProperty(c => c.AuthorIp, "127.0.0.1")
                   .CheckProperty(c => c.Body, "<p>test spam comment</p>")
                   .CheckProperty(c => c.Posted, DateTime.Now)
                   .CheckProperty(c => c.Status, CommentStatus.Spam)
                   .CheckProperty(c => c.IsSpam, true)
                   .CheckProperty(c => c.RevisionNumber, 1)
                   .CheckReference(c => c.Snippet, snippet1)
                   .VerifyTheMappings();
            }
        }
    }
}
