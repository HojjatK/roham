using Roham.Domain.Entities;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetRevisionMapping : IdentifiableMap<SnippetRevision>
    {
        public SnippetRevisionMapping()
        {
            Map(x => x.RevisionNumber);
            Map(x => x.Summary);            
            Map(x => x.Author);            
            Map(x => x.RevisedDate);
            Map(x => x.ReviseReason);            
            Map(x => x.BodyEncoding);            
            Map(x => x.Body);
            Map(x => x.BodyImage);

            References(x => x.Snippet, MappingNames.RefId<Snippet>());
            References(x => x.Reviser, MappingNames.ReviserId);
        }
    }
}
