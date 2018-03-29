using FluentNHibernate;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Persistence.NHibernate.UserTypes;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Sites
{
    public class SiteMapping : AggregateRootMap<Site>
    {
        public SiteMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.IsDefault);
            Map(x => x.IsActive);
            Map(x => x.IsPrivate);

            References(x => x.Owner).Column(MappingNames.OwnerId);
            References(x => x.Portal).Column(MappingNames.RefId<Portal>());

            var siteIdColumnName = MappingNames.RefId<Site>();
            HasManyToMany(x => x.Users)
                .AsSet()
                .Table(MappingNames.SiteUserMapTableName)
                .ParentKeyColumn(siteIdColumnName)
                .ChildKeyColumn(MappingNames.RefId<User>())
                .Cascade.None();

            HasMany(x => x.Settings)
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasMany(x => x.WorkflowRules)
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();
            
            HasMany<Zone>(Reveal.Member<Site>(Site.NameOfZones))
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasMany<Tag>(Reveal.Member<Site>(Site.NameOfTags))
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasMany<Job>(Reveal.Member<Site>(Site.NameOfJobs))
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasMany<PostSerie>(Reveal.Member<Site>(Site.NameOfPostSeries))
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasMany<Post>(Reveal.Member<Site>(Site.NameOfEntries))
                .AsSet()
                .KeyColumn(siteIdColumnName)
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}
