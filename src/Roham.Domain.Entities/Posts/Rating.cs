using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Rating domain entity.
    /// </summary>
    public class Rating : EntryRating
    {
        [Required]
        public virtual Post Post { get; set; }
    }
}
