using FluentNHibernate;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Posts
{
    public class TagMapping : AggregateRootMap<Tag>
    {
        public TagMapping()
        {
            Map(x => x.Name);            

            References(x => x.Site).Column(MappingNames.RefId<Site>());

            HasManyToMany<Post>(Reveal.Member<Tag>(Tag.NameOfEntries))
                .AsSet()
                .Table(MappingNames.TagPostMapTableName)
                .ParentKeyColumn(MappingNames.RefId<Tag>())
                .ChildKeyColumn(MappingNames.RefId<Post>())
                .Inverse()
                .Cascade.None();
        }
    }
}
