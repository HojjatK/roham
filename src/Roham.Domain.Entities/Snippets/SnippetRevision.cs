using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Snippets
{
    /// <summary>
    /// Code snippet entry revision domain entity.
    /// </summary>
    public class SnippetRevision : EntryRevision
    {
        [MaxLength(Lengths.Name)]
        public virtual string Theme { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Language { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual bool LineNumbers { get; set; }


        [MaxLength(Lengths.Description)]
        public virtual string DataLine { get; set; }

        [Required]
        public virtual Snippet Snippet { get; set; }
    }
}
