using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Entries
{
    /// <summary>
    /// Abstract entry revision domain entity.
    /// </summary>
    public abstract class EntryRevision : Identifiable
    {   
        protected EntryRevision()
        {
        }

        [Required]
        public virtual int RevisionNumber { get; set; }

        [MaxLength(Lengths.EntryContent)]
        public virtual string Summary { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Author { get; set; }

        [Required]
        public virtual User Reviser { get; set; }

        [Required]
        public virtual DateTime RevisedDate { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string ReviseReason { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string BodyEncoding { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a body for this entry.")]
        [MaxLength(int.MaxValue)]
        public virtual string Body { get; set; }

        public virtual byte[] BodyImage { get; set; }
    }
}
