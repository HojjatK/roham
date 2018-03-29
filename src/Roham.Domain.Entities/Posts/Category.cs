using Roham.Lib.Domain;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Category domain entity.
    /// </summary>
    public class Category : Tag
    {
        [Required]
        public virtual bool IsPrivate { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        public virtual Category Parent { get; set; }
    }
}
