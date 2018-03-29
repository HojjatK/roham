/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Domain.Entities;
using Roham.Domain.Entities.Jobs;

namespace Roham.Persistence.NHibernate.Mappings.Jobs
{
    public class JobTaskMapping : AggregateRootMap<JobTask>
    {
        public JobTaskMapping()
        {
            Map(x => x.Name);
            Map(x => x.Status);
            Map(x => x.ProgressEstimate);
            Map(x => x.Created);
            Map(x => x.Completed);
            Map(x => x.FailedMessage);
            Map(x => x.OwnerUserName);

            References(x => x.Job).Column(MappingNames.RefId<Job>());
            HasMany(x => x.Details)
                .AsSet()
                .Table(MappingNames.JobTaskHistoryTableName)
                .KeyColumn(MappingNames.RefId<JobTask>())
                .Inverse()
                .Cascade.All();
        }
    }
}