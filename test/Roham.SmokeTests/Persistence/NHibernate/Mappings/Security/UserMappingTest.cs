using System;
using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class UserMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenUserEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestUserMapping()
            {
                // given
                var person1 = GetOrCreatePerson("test", "user", "test address", "11223344");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog");
                var user1 = GetOrCreateUser("test.user");

                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                Session.Save(post1);
                Session.Flush();

                // assert                
                new PersistenceSpecification<User>(Session, new CustomEqualityComparer())
                    .CheckProperty(u => u.Uid, Guid.NewGuid())
                    .CheckProperty(u => u.UserName, "test user name 64")
                    .CheckProperty(u => u.Email, "a@test64.com")
                    .CheckProperty(u => u.EmailConfirm, true)
                    .CheckProperty(u => u.PasswordHashAlgorithm, "MD5")
                    .CheckProperty(u => u.PasswordHash, "098f6bcd4621d373cade4e832627b4f6")
                    .CheckProperty(u => u.SecurityStamp, "MDk4ZjZiY2Q0NjIxZDM3M2NhZGU0ZTgzMjYyN2I0ZjY=")
                    .CheckProperty(u => u.PhoneNumber, "12345678")
                    .CheckProperty(u => u.PhoneNumberConfirmed, true)
                    .CheckProperty(u => u.TwoFactorEnabled, true)
                    .CheckProperty(u => u.LockoutEndDateUtc, DateTime.Now.AddMinutes(5))
                    .CheckProperty(u => u.LockoutEnabled, true)
                    .CheckProperty(u => u.AccessFailedCount, 5)
                    .CheckProperty(u => u.IsSystemUser, false)
                    .CheckProperty(u => u.StatusReason, "")
                    .CheckProperty(u => u.Status, UserStatus.Active)
                    .CheckProperty(u => u.Party, person1)
                    .CheckInverseList(u => u.UserClaims, new List<UserClaim> {
                        new UserClaim { ClaimType = "ctype1", ClaimValue = "value1" },
                        new UserClaim { ClaimType = "ctype2", ClaimValue = "value2" } },
                        (u, uc) => { uc.User = u; u.UserClaims.Add(uc); })
                    .CheckInverseList(u => u.UserLogins, new List<UserLogin> {
                        new UserLogin { LoginProvider = "Facebook", ProviderKey = "9C98E019-4DA1-4176-AF72-3033841E6CC6" },
                        new UserLogin { LoginProvider = "Twitter", ProviderKey = "19071494-AB47-42B3-99E0-DF8D66B9EF8D" } },
                        (u, uo) => { uo.User = u; u.UserLogins.Add(uo); })
                    .CheckList(u => u.Roles, new List<Role> { new Role { Name = "Test role", Description = "Test Role Description" } }, (u, r) => u.Roles.Add(r))
                    .CheckInverseList(u => u.Permissions, new List<PostPermission> { new PostPermission { Create = true, Update = true } }, (u, p) => { p.User = u; p.Post = post1; u.Permissions.Add(p); })
                    .VerifyTheMappings();
            }
        }
    }
}