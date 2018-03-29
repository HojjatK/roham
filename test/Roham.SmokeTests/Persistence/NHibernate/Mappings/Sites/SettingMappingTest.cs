using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Sites;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.Mappings.Sites
{
    public class SettingMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenSettingEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestSettingMapping()
            {
                var site1 = GetOrCreateSite("site1");

                new PersistenceSpecification<Setting>(Session, new CustomEqualityComparer())
                   .CheckProperty(u => u.Uid, Guid.NewGuid())
                   .CheckProperty(s => s.Section, new PageName("test.setting.section"))
                   .CheckProperty(s => s.Name, new PageName("test.setting.name"))
                   .CheckProperty(s => s.Title, "test setting name")
                   .CheckProperty(s => s.Description, "setting description")
                   .CheckProperty(s => s.Value, "setting value")
                   .CheckProperty(s => s.Site, site1)
                   .VerifyTheMappings();
            }
        }
    }
}
