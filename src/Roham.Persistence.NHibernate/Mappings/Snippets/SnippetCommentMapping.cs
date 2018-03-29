using Roham.Domain.Entities;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetCommentMapping : AggregateRootMap<SnippetComment>
    {
        public SnippetCommentMapping()
        {
            Map(x => x.AuthorName);
            Map(x => x.AuthorUrl);
            Map(x => x.AuthorEmail);
            Map(x => x.AuthorIp);
            Map(x => x.Body);
            Map(x => x.Posted);
            Map(x => x.Status);
            Map(x => x.RevisionNumber);

            References(x => x.Snippet, MappingNames.RefId<Snippet>());
        }
    }
}
