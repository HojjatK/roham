using System;
using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class RoleMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenRoleEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestRoleMapping()
            {
                // given                
                var user1 = GetOrCreateUser("user1");
                var site1 = GetOrCreateSite("site1");
                var zone1 = GetOrCreateZone(site1, "blog");
                var post1 = TestDataBuilder.NewPost(site1, zone1, user1);
                var post2 = TestDataBuilder.NewPost(site1, zone1, user1);

                // assert
                new PersistenceSpecification<Role>(Session, new CustomEqualityComparer())
                   .CheckProperty(u => u.Uid, Guid.NewGuid())
                   .CheckProperty(r => r.Name, "Role Name")
                   .CheckProperty(r => r.Description, "Role Description")
                   .CheckProperty(r => r.RoleType, RoleTypeCodes.Administrator)
                   .CheckProperty(r => r.IsSystemRole, true)
                   .CheckList(r => r.AppFunctions, new List<AppFunction> {
                       new AppFunction { Key = FunctionKeys.CreatePost, Title = "Create Post" },
                       new AppFunction { Key = FunctionKeys.CreateCategory, Title = "Create Category"  }, }, (r, p) => { r.AppFunctions.Add(p); })
                   .VerifyTheMappings();
            }
        }
    }
}
