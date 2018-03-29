using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Post entry's comment domain entity.
    /// </summary>
    public class Comment : EntryComment
    {
        [Required]
        public virtual Post Post { get; set; }
    }
}
