using System;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Roham.Domain.Entities.Jobs;
using System.Collections.Generic;

namespace Roham.Persistence.NHibernate.Mappings.Jobs
{
    public class JobTaskMappingTest {

        [TestFixture]
        [Category("SmokeTests.NHibernate.Mapping")]
        internal class GivenJobTaskEntity : NHibernateEntityMappingFixture
        {
            [Test]
            public void TestJobTaskMapping()
            {
                var user1 = GetOrCreateUser("test.job.user");
                var site1 = GetOrCreateSite("site1");
                DateTime baseTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(5));
                var job1 = TestDataBuilder.Build<Job>()
                    .With(j => j.Site, site1)
                    .With(j => j.Owner, user1)
                    .Create();
                Session.Save(job1);
                Session.Flush();

                // assert
                new PersistenceSpecification<JobTask>(Session, new CustomEqualityComparer())
                    .CheckProperty(x => x.Name, "Job1")
                    .CheckProperty(x => x.Status, TaskStatus.InProgress)
                    .CheckProperty(x => x.ProgressEstimate, 18)
                    .CheckProperty(x => x.Created, DateTime.Now.Subtract(TimeSpan.FromSeconds(45)))
                    .CheckProperty(x => x.Completed, DateTime.Now.Add(TimeSpan.FromSeconds(45)))
                    .CheckProperty(x => x.FailedMessage, "failed")
                    .CheckProperty(x => x.OwnerUserName, "another.user")
                    .CheckInverseList(x => x.Details, new List<JobTaskDetail> {
                       new JobTaskDetail { TryNo = 1, Started = baseTime.Add(TimeSpan.FromSeconds(10)), Updated = baseTime.Add(TimeSpan.FromSeconds(20)), Finished = baseTime.Add(TimeSpan.FromSeconds(30)), Status = TaskStatus.Failed },
                       new JobTaskDetail { TryNo = 2, Started = baseTime.Add(TimeSpan.FromSeconds(40)), Updated = baseTime.Add(TimeSpan.FromSeconds(60)), Finished = baseTime.Add(TimeSpan.FromSeconds(60)), Status = TaskStatus.Succeed}
                    }, (j, d) => { d.OwnerTask = j; j.Details.Add(d); });
            }
        }

    }
}