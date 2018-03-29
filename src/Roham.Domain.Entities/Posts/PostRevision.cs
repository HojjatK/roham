using System;
using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Entries;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Posts
{
    /// <summary>
    /// Post revision domain entity.
    /// </summary>
    public class PostRevision : EntryRevision
    {
        [MaxLength(Lengths.LongDescription)]
        public virtual string TagsCommaSeperated { get; set; }

        public virtual DateTime? PublishedDate { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string PublisherRoleName { get; set; }

        public virtual DateTime? ApprovedDate { get; set; }

        public virtual User Approver { get; set; }

        public virtual User Publisher { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string ApproverRoleName { get; set; }

        [Required]
        public virtual ContentFormats Format { get; set; }

        public virtual long ViewsCount { get; set; }

        [Required]
        public virtual Post Post { get; set; }
    }
}