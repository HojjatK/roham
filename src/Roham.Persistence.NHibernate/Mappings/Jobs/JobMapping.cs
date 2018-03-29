using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Jobs
{
    public class JobMapping : AggregateRootMap<Job>
    {
        public JobMapping()
        {
            Map(x => x.Name);
            Map(x => x.Type);
            Map(x => x.IsSystemJob);
            Map(x => x.Description);            
            Map(x => x.CronPattern);            
            Map(x => x.Created);            

            References(x => x.Owner, MappingNames.OwnerId);
            References(x => x.Site).Column(MappingNames.RefId<Site>());

            HasMany(x => x.Tasks)
                .AsSet()
                .Table(MappingNames.JobTaskTableName)
                .KeyColumn(MappingNames.RefId<Job>())
                .Inverse()
                .Cascade.All();
        }
    }
}
