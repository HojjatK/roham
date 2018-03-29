using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class PostWorkflowRuleMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenXEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestWorkflowMapping()
            {
                var site1 = GetOrCreateSite("site1");
                var role1 = GetOrCreateAdminRole();

                new PersistenceSpecification<PostWorkflowRule>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, "test workflow mapping")
                    .CheckProperty(x => x.IsActive, true)
                    .CheckProperty(x => x.ReturnToAuthorForPublish, true)
                    .CheckReference(x => x.ApproverRole, role1)
                    .CheckReference(x => x.PublisherRole, role1)
                    .CheckReference(x => x.Site, site1)
                    .VerifyTheMappings();
            }
        }
    }
}
