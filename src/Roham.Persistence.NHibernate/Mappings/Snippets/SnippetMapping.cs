using Roham.Domain.Entities;
using Roham.Domain.Entities.Filters;
using Roham.Domain.Entities.Snippets;
using Roham.Persistence.NHibernate.UserTypes;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetMapping : AggregateRootMap<Snippet>
    {
        public SnippetMapping()
        {
            Map(x => x.Name).CustomType<PageNameUserType>();
            Map(x => x.Title);
            Map(x => x.MimeType);
            Map(x => x.Author);
            Map(x => x.CommentsCount);
            Map(x => x.Rating);
            Map(x => x.DisableDiscussionDays);
            Map(x => x.IsDiscussionEnabled);
            Map(x => x.IsRatingEnabled);
            Map(x => x.IsAnonymousCommentAllowed);
            Map(x => x.Created);
            Map(x => x.IsPrivate);
            Map(x => x.IsContentBinary);

            Component(x => x.LatestRevision,
                c =>
                {
                    c.Map(r => r.Id, MappingNames.LastRevisionId);
                    c.Map(r => r.RevisionNumber, MappingNames.RevisionNumber);
                    c.Map(r => r.Author).Column(MappingNames.RevisionAuthor);
                    c.Map(r => r.Body).Length(int.MaxValue);
                    c.Map(r => r.BodyImage);
                    c.Map(r => r.RevisedDate).Column(MappingNames.LastRevised);
                    c.Map(r => r.BodyEncoding).Column(MappingNames.LatestRevisionEncoding);
                });

            References(x => x.Creator, MappingNames.CreatorId);

            var snippetIdColumnName = MappingNames.RefId<Snippet>();
            HasMany(x => x.Revisions)
                .AsSet()
                .KeyColumn(snippetIdColumnName)
                .Inverse()
                .LazyLoad()
                .ApplyFilter<RevisionFilter>($"{MappingNames.RevisionNumber} = :revisionNumber")
                .Cascade.All();

            HasMany(x => x.Comments)
                .AsSet()
                .KeyColumn(snippetIdColumnName)
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Ratings)
                .AsSet()
                .KeyColumn(snippetIdColumnName)
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Links)
                .AsSet()
                .KeyColumn(snippetIdColumnName)
                .Inverse()
                .Cascade.All();

            HasMany(x => x.Pingbacks)
                .AsSet()
                .KeyColumn(snippetIdColumnName)
                .Inverse()
                .Cascade.All();
        }
    }
}
