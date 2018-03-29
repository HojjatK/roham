using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Parties;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Sites
{   
    /// <summary>
    /// Portal domain entity.
    /// </summary>
    public class Portal : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Portal_Name")]
        [MaxLength(Lengths.Name)]
        public virtual PageName Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [StringLength(Lengths.Description)]
        public virtual string Description { get; set; }

        [Required]
        public virtual User Owner { get; set; }

        public virtual Organisation Organisation { get; set; }

        private ICollection<Site> _sites;
        public virtual ICollection<Site> Sites
        {
            get { return this.LazySet(ref _sites); }
            protected set { _sites = value.AsSet(); }
        }
    }
}