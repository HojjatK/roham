using Roham.Domain.Entities.Filters;
using Roham.Domain.Entities.Sites;
using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Domain.Entities.Posts
{
    public class PostMapping : AggregateRootMap<Post>
    {
        public PostMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.Title);
            Map(x => x.MetaTitle);
            Map(x => x.MetaDescription);
            Map(x => x.MimeType);
            Map(x => x.Format);
            Map(x => x.Author);
            Map(x => x.CommentsCount);
            Map(x => x.ViewsCount);
            Map(x => x.PageTemplate);            
            Map(x => x.Status);
            Map(x => x.Created);
            Map(x => x.PublishDate);
            Map(x => x.EffectiveDate);
            Map(x => x.DisableDiscussionDays);
            Map(x => x.Rating);
            Map(x => x.Popularity);
            Map(x => x.IsDiscussionEnabled);
            Map(x => x.IsRatingEnabled);
            Map(x => x.IsPrivate);
            Map(x => x.IsChromeHidden);
            Map(x => x.IsContentBinary);
            Map(x => x.IsPingbackEnabled);
            Map(x => x.IsTrackbackEnabled);
            Map(x => x.IsAnonymousCommentAllowed);

            Component(x => x.LatestRevision,
                c =>
                {
                    c.Map(r => r.Id, MappingNames.LastRevisionId);
                    c.Map(r => r.RevisionNumber).Column(MappingNames.RevisionNumber);
                    c.Map(r => r.Author).Column(MappingNames.RevisionAuthor);
                    c.Map(r => r.Body).Length(int.MaxValue);
                    c.Map(r => r.BodyImage);
                    c.Map(r => r.RevisedDate).Column(MappingNames.LastRevised);
                    c.Map(r => r.BodyEncoding).Column(MappingNames.LatestRevisionEncoding);
                    c.Map(r => r.Format).Column(MappingNames.LatestRevisionFormat);
                });

            References(x => x.Zone, MappingNames.RefId<Zone>());
            References(x => x.Serie, MappingNames.RefId<PostSerie>());
            References(x => x.Creator, MappingNames.CreatorId);
            References(x => x.Site).Column(MappingNames.RefId<Site>());

            var postIdColumnName = MappingNames.RefId<Post>();
            HasMany(x => x.Revisions)
                .AsSet()
                .KeyColumn(MappingNames.RefId<Post>())
                .Inverse()
                .LazyLoad()
                .ApplyFilter<RevisionFilter>($"{MappingNames.RevisionNumber} = :revisionNumber")
                .Cascade.All();

            HasMany(x => x.Comments)
                .AsSet()
                .KeyColumn(postIdColumnName)
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Ratings)
                .AsSet()
                .KeyColumn(postIdColumnName)
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Links)
                .AsSet()
                .KeyColumn(postIdColumnName)
                .Inverse()
                .Cascade.All();

            HasManyToMany(x => x.Tags)
                .AsSet()
                .Table(MappingNames.TagPostMapTableName)
                .ParentKeyColumn(postIdColumnName)
                .ChildKeyColumn(MappingNames.RefId<Tag>())
                .Cascade.None();

            HasMany(x => x.Permissions)
               .AsSet()
               .KeyColumn(postIdColumnName)
               .Inverse()
               .LazyLoad()
               .Cascade.All();

            HasMany(x => x.Pingbacks)
                .AsSet()
                .KeyColumn(postIdColumnName)
                .Inverse()
                .Cascade.All();
        }
    }
}
