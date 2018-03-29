using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Snippets;
using System;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetRatingMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenSnippetRatingEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSnippetRatingMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.moderator1");                
                var snippet1 = TestDataBuilder.NewSnippet(user1);
                Session.Save(snippet1);
                Session.Flush();

                // assert
                new PersistenceSpecification<SnippetRating>(Session, new CustomEqualityComparer())
                   .CheckProperty(p => p.Rate, (decimal)2.3)
                   .CheckProperty(p => p.RatedDate, DateTime.Now)
                   .CheckProperty(p => p.UserIdentity, "pingback.user")
                   .CheckProperty(p => p.UserEmail, "pingback.user@emial.com")
                   .CheckReference(p => p.Snippet, snippet1)
                   .VerifyTheMappings();
            }
        }
    }
}
