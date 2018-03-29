using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Sites;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class RedirectMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenRedirectEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestRedirectMapping()
            {
                new PersistenceSpecification<Redirect>(Session, new CustomEqualityComparer())
                   .CheckProperty(u => u.Uid, Guid.NewGuid())
                   .CheckProperty(s => s.From, "http://www.from.url.com")
                   .CheckProperty(s => s.To, "http://www.to.url.com")
                   .VerifyTheMappings();
            }
        }
    }
}
