using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Post serie domain entity.
    /// </summary>
    public class PostSerie : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        [Unique("UQ_Site_PostSerie_Name")]
        public virtual PageName Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool IsPrivate { get; set; }

        [Unique("UQ_Site_PostSerie_Name")]
        public virtual Site Site { get; set; }

        private ICollection<Post> _posts;
        public virtual ICollection<Post> Posts
        {
            get { return this.LazySet(ref _posts); }
            protected set { _posts = value.AsSet(); }
        }
    }
}