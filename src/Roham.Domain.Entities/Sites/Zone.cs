using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Posts;

namespace Roham.Domain.Entities.Sites
{
    /// <summary>
    /// Zone domain entity.
    /// </summary>
    public class Zone : AggregateRoot, INamed
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a name.")]
        [MaxLength(Lengths.Name)]
        [Unique("UQ_Site_Zone_Name")]
        public virtual PageName Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a title.")]
        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual bool IsPrivate { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        [Required]
        [Unique("UQ_Site_Zone_Name")]
        public virtual Site Site { get; set; }

        [Required]
        public virtual ZoneTypeCodes ZoneType { get; set; }

        private ICollection<Post> _entries;
        public virtual ICollection<Post> Entries
        {
            get { return this.LazySet(ref _entries); }
            protected set { _entries = value.AsSet(); }
        }
    }
}
