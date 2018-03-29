using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Posts;

namespace Roham.Persistence.NHibernate.Mappings.Posts
{
    public class CategoryMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenCategoryEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestCategoryMapping()
            {
                // arrange 
                var site1 = GetOrCreateSite("site1");

                // assert
                new PersistenceSpecification<Category>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Parent, null)
                    .CheckProperty(x => x.Name, "test category")
                    .CheckProperty(x => x.IsCategory, true)
                    .CheckProperty(x => x.Description, "test category description")
                    .CheckProperty(x => x.IsPrivate, false)
                    .CheckReference(x => x.Site, site1);
            }

            [Test]
            public void TestChildCategoryMapping()
            {
                // arrange 
                var site1 = GetOrCreateSite("site1");
                var parentCategory = GetOrCreateCategory(site1, "test parent category");

                // assert
                new PersistenceSpecification<Category>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Parent, parentCategory)
                    .CheckProperty(x => x.Name, "test child category")
                    .CheckProperty(x => x.IsCategory, true)
                    .CheckProperty(x => x.Description, "test child category description")
                    .CheckProperty(x => x.IsPrivate, true)
                    .CheckReference(x => x.Site, site1);
            }
        }
    }
}
