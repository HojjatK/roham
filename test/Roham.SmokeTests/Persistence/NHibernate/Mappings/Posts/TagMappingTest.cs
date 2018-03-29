using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class TagMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenTagEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestTagMapping()
            {
                // given
                var site1 = GetOrCreateSite("site1");

                // assert
                new PersistenceSpecification<Tag>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, "test tag")
                    .CheckProperty(x => x.IsCategory, false)
                    .CheckReference(x => x.Site, site1);
            }
        }
    }
}
