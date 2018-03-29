using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Snippets
{
    /// <summary>
    /// Code snippet rating domain entity.
    /// </summary>
    public class SnippetRating : EntryRating
    {
        [Required]
        public virtual Snippet Snippet { get; set; }
    }
}
