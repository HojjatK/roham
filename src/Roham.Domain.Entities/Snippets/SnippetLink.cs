using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Snippets
{
    /// <summary>
    /// Code snippet entry link domain entity.
    /// </summary>
    public class SnippetLink : EntryLink
    {
        [Required]
        public virtual Snippet Snippet { get; set; }
    }
}
