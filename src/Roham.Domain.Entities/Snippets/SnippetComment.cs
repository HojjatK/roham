using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Snippets
{
    /// <summary>
    /// Code snippet entry comment domain entity.
    /// </summary>
    public class SnippetComment : EntryComment
    {
        [Required]
        public virtual Snippet Snippet { get; set; }
    }
}
