using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetLinkMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostLinkEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSnippetLinkMapping()
            {
                var user1 = GetOrCreateUser("user1");
                // given
                var snippet1 = TestDataBuilder.NewSnippet(user1);

                Session.Save(snippet1);
                Session.Flush();

                // assert
                new PersistenceSpecification<SnippetLink>(Session, new CustomEqualityComparer())
                   .CheckProperty(p => p.Type, "test/css")
                   .CheckProperty(p => p.Ref, "http://www.testdomain.com.au/test/testlink.css")
                   .CheckReference(p => p.Snippet, snippet1)
                   .VerifyTheMappings();
            }
        }
    }
}
