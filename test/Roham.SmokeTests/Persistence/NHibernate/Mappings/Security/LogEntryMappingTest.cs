using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Security;

namespace Roham.Persistence.NHibernate.Mappings.Security
{

    public class LogEntryMappingTest 
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenLogEntryMappingEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestLogEntryMapping()
            {
                // assert
                new PersistenceSpecification<LogEntry>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Timestamp, DateTime.Now)
                    .CheckProperty(x => x.Level, LogEntryLevel.Error)
                    .CheckProperty(x => x.Message, "Test Error")
                    .CheckProperty(x => x.LoggerName, "TestLogger")
                    .CheckProperty(x => x.Thread, 1.ToString())
                    .CheckProperty(x => x.ProcessUser, "IdentityUser")
                    .CheckProperty(x => x.SessionToken, Guid.NewGuid().ToString())
                    .CheckProperty(x => x.SessionUser, "Administrator")
                    .CheckProperty(x => x.StackTrace, "method1\r\nmethod2\r\nmethod3()")
                    .CheckProperty(x => x.ClientStackTrace, "clientmethod1\r\nclientmethod2\r\nclientmethod3()")
                    .CheckProperty(x => x.Exception, "NullReferenceException")
                    .CheckProperty(x => x.Extra, "extra")
                    .VerifyTheMappings();
            }
        }
    }
}
