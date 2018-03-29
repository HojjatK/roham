using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{
    public class AppFunctionMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenAppFunctionEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestAppFunctionMapping()
            {
                // given
                var parentAction = new AppFunction
                {
                    Key = FunctionKeys.Sites,
                    Title = "Parent Action Translation Key"
                };
                Session.Save(parentAction);
                Session.Flush();

                // assert
                new PersistenceSpecification<AppFunction>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Key, FunctionKeys.Sites)
                    .CheckProperty(x => x.Title, "func-name")
                    .CheckProperty(x => x.Title, "func title")
                    .CheckProperty(x => x.Description, "func description")
                    .CheckReference(x => x.Parent, parentAction);
            }
        }
    }
}
