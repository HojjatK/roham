using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class UserClaimMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenUserClaimEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestUserLoginMapping()
            {
                // given
                var user = GetOrCreateUser("test.userclaim.user1");

                // assert
                new PersistenceSpecification<UserClaim>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.ClaimType, "test-claim-type")
                    .CheckProperty(x => x.ClaimValue, "test-claim-value")
                    .CheckReference(x => x.User, user)
                    .VerifyTheMappings();
            }
        }
    }
}
