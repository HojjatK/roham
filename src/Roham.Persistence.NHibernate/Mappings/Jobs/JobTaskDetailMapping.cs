/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Domain.Entities;
using Roham.Domain.Entities.Jobs;

namespace Roham.Persistence.NHibernate.Mappings.Jobs
{
    public class JobTaskDetailMapping : IdentifiableMap<JobTaskDetail>
    {
        public JobTaskDetailMapping()
        {
            Map(x => x.Arguments);
            Map(x => x.TryNo);
            Map(x => x.Status);
            Map(x => x.Started);
            Map(x => x.Updated);
            Map(x => x.Finished);
            Map(x => x.OutputLog);

            References(x => x.OwnerTask, MappingNames.RefId<JobTask>());
        }
    }
}