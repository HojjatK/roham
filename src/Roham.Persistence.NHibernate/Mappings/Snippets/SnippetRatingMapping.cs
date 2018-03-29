using Roham.Domain.Entities;
using Roham.Domain.Entities.Snippets;

namespace Roham.Persistence.NHibernate.Mappings.Snippets
{
    public class SnippetRatingMapping : IdentifiableMap<SnippetRating>
    {
        public SnippetRatingMapping()
        {
            Map(x => x.Rate);
            Map(x => x.RatedDate);
            Map(x => x.UserIdentity);
            Map(x => x.UserEmail);

            References(x => x.Snippet, MappingNames.RefId<Snippet>());
        }
    }
}
