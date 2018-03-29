using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetRevisionMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostRevision : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSnippetRevisionMapping()
            {
                var user1 = GetOrCreateUser("test.moderator1");
                var snippet1 = TestDataBuilder.NewSnippet(user1, 10, 2);

                Session.Save(snippet1);
                Session.Flush();

                // assert
                new PersistenceSpecification<SnippetRevision>(Session, new CustomEqualityComparer())
                    .CheckProperty(e => e.RevisionNumber, 1)
                    .CheckProperty(e => e.Summary, "test summary")                    
                    .CheckProperty(e => e.Author, "test")                    
                    .CheckProperty(e => e.RevisedDate, DateTime.Now.AddDays(-2))
                    .CheckProperty(e => e.ReviseReason, "test reason")
                    .CheckProperty(e => e.Body, "for(int i = 1; i < 10; i++){\r\nConsole.WriteLine(\"Test\");}\r\n")
                    .CheckReference(e => e.Snippet, snippet1)
                    .CheckReference(e => e.Reviser, user1)                    
                    .VerifyTheMappings();
            }
        }
    }
}
