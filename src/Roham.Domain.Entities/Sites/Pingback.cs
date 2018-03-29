using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Snippets;

namespace Roham.Domain.Entities.Sites
{
    /// <summary>
    /// Pingback domain entity.
    /// </summary>
    public class Pingback : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Location)]
        public virtual string TargetUri { get; set; }

        [MaxLength(Lengths.Location)]
        public virtual string TargetTitle { get; set; }

        [Required]
        public virtual bool IsSpam { get; set; }

        [Required]
        public virtual bool IsTrackback { get; set; }

        [Required]
        public virtual DateTime Received { get; set; }

        public virtual Post Post { get; set; }

        public virtual Snippet Snippet { get; set; }
    }
}
