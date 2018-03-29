using System;
using FluentNHibernate.Testing;
using NUnit.Framework;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class UserSessionMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenUserSessionEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSessionMapping()
            {
                var user = GetOrCreateUser("test.user");

                // assert                
                new PersistenceSpecification<Roham.Domain.Entities.Security.UserSession>(Session, new CustomEqualityComparer())
                .CheckProperty(x => x.Uid, Guid.NewGuid())
                .CheckProperty(x => x.Status, Roham.Domain.Entities.Security.SessionStatus.Active)
                .CheckProperty(x => x.StartTimestamp, DateTime.Now)
                .CheckProperty(x => x.EndTimestamp, DateTime.Now.AddMinutes(30))
                .CheckReference(x => x.User, user)
                .VerifyTheMappings();
            }
        }
    }
}
