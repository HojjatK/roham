using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class PostPermissionMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenPostPermissionEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestPostPermissionMapping()
            {
                // given
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog");
                var user1 = GetOrCreateUser("test.user");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);

                // assert
                new PersistenceSpecification<PostPermission>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Read, true)
                    .CheckProperty(x => x.Create, true)
                    .CheckProperty(x => x.Update, true)
                    .CheckProperty(x => x.Delete, false)
                    .CheckProperty(x => x.Execute, false)
                    .CheckReference(x => x.User, user1)
                    .CheckReference(x => x.Post, post1)
                    .VerifyTheMappings();
            }
        }
    }
}
