using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class UserLoginMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenUserLoginEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestUserLoginMapping()
            {
                // given
                var user = GetOrCreateUser("test.userlogin.user1");

                // assert
                new PersistenceSpecification<UserLogin>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.LoginProvider, "test.provider")
                    .CheckProperty(x => x.ProviderKey, "test-provider-key")
                    .CheckReference(x => x.User, user)
                    .VerifyTheMappings();
            }
        }
    }
}
