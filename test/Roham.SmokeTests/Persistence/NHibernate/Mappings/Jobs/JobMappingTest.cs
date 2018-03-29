using System;
using System.Collections.Generic;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Roham.Domain.Entities.Jobs;

namespace Roham.Persistence.NHibernate.Mappings.Jobs
{
    public class JobMappingTest
    {
        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenJobEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestJobMapping()
            {
                // given
                var user1 = GetOrCreateUser("test.job.user");
                var site1 = GetOrCreateSite("site1");
                DateTime baseTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));

                // assert
                new PersistenceSpecification<Job>(Session, new CustomEqualityComparer())
                   .CheckProperty(x => x.Name, "Job1")
                   .CheckProperty(x => x.Type, JobType.ExportEntries)
                   .CheckProperty(x => x.IsSystemJob, false)
                   .CheckProperty(x => x.Description, "test description")
                   .CheckProperty(x => x.CronPattern, "0 0 12 1/1 * ? *")
                   .CheckProperty(x => x.Created, DateTime.Now.Subtract(TimeSpan.FromSeconds(45))) 
                   .CheckReference(x => x.Owner, user1)
                   .CheckReference(x => x.Site, site1)
                   .CheckInverseList(x => x.Tasks, new List<JobTask> {
                       new JobTask { Name = "job1 - 1" , Created = baseTime.Add(TimeSpan.FromSeconds(10)), Status = TaskStatus.Failed, },
                       new JobTask { Name = "job1 - 2" , Created = baseTime.Add(TimeSpan.FromSeconds(40)), Completed = baseTime.Add(TimeSpan.FromSeconds(60)), Status = TaskStatus.Succeed}
                       }, (j, d) => { d.Job = j; j.Tasks.Add(d); })
                   .VerifyTheMappings();
            }
        }
    }
}
