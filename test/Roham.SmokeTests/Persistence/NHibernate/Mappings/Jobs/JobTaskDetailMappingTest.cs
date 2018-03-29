using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Roham.Domain.Entities.Jobs;

namespace Roham.Persistence.NHibernate.Mappings.Jobs
{
    public class JobDetailMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenJobTaskDetailEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestJobDetailMapping()
            {
                // given                
                var user1 = GetOrCreateUser("test.job.user");
                var site1 = GetOrCreateSite("site1");
                DateTime baseTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
                var job1 = TestDataBuilder.Build<Job>()
                    .With(j => j.Site, site1)
                    .With(j => j.Owner, user1)
                    .Create();
                Session.Save(job1);
                Session.Flush();
                var task1 = TestDataBuilder.Build<JobTask>()
                    .With(j => j.Name, job1.Name)                    
                    .With(j => j.Job, job1)
                    .Create();

                // assert
                new PersistenceSpecification<JobTaskDetail>(Session, new CustomEqualityComparer())                    
                    .CheckProperty(x => x.TryNo, 2)
                    .CheckProperty(x => x.Arguments, "test job details arguments")
                    .CheckProperty(x => x.Status, TaskStatus.Failed)
                    .CheckProperty(x => x.Started, baseTime)
                    .CheckProperty(x => x.Updated, baseTime.AddMinutes(3))
                    .CheckProperty(x => x.Finished, baseTime.AddMinutes(3))
                    .CheckProperty(x => x.OutputLog, "test output log")
                    .CheckReference(x => x.OwnerTask, task1)
                    .VerifyTheMappings();

                
            }
        }
    }
}
