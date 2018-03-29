using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Sites
{
    /// <summary>
    /// Redirect domain entity.
    /// </summary>
    public class Redirect : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Location)]
        public virtual string From { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Location)]
        public virtual string To { get; set; }

        [Required]
        public virtual DateTime Timestamp { get; set; }
    }
}
