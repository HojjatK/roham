using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Post entry link.
    /// </summary>
    public class PostLink : EntryLink
    {
        [Required]
        public virtual Post Post { get; set; }
    }
}
