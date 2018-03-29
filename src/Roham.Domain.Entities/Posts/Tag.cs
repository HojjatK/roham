using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Tag domain entity
    /// </summary>
    public class Tag : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Site_Tag_Name")]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        // is not mapped
        public virtual bool IsCategory
        {
            get { return this is Category; }
            protected set { }
        }

        [Required]
        [Unique("UQ_Site_Tag_Name")]
        public virtual Site Site { get; set; }

        public virtual IEnumerable<Post> GetEntries()
        {
            return Entries.AsEnumerable();
        }

        private ICollection<Post> _entries;
        protected virtual ICollection<Post> Entries
        {
            get { return this.LazySet(ref _entries); }
            set { _entries = value.AsSet(); }
        }

        public static string NameOfEntries => nameof(Entries);
    }
}
