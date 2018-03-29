using Roham.Domain.Entities;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetLinkMapping : IdentifiableMap<SnippetLink>
    {
        public SnippetLinkMapping()
        {
            Map(x => x.Type);
            Map(x => x.Ref);

            References(x => x.Snippet, MappingNames.RefId<Snippet>());
        }
    }
}
