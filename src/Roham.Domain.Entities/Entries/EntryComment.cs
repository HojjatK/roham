using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Entries
{
    public enum CommentStatus
    {
        Spam = 0,
        NotSpam = 1,
    }

    /// <summary>
    /// Abstract entry comment domain entity.
    /// </summary>
    public abstract class EntryComment : AggregateRoot        
    {
        protected EntryComment()
        {
        }

        [Required]
        [MaxLength(Lengths.Name)]
        public virtual string AuthorName { get; set; }

        [MaxLength(Lengths.Location)]
        public virtual string AuthorUrl { get; set; }

        [MaxLength(Lengths.Email)]
        public virtual string AuthorEmail { get; set; }

        [MaxLength(Lengths.ShortName)]
        public virtual string AuthorIp { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(int.MaxValue)]
        public virtual string Body { get; set; }

        [Required]
        public virtual DateTime Posted { get; set; }

        [Required]
        public virtual CommentStatus Status { get; set; }

        public virtual bool IsSpam
        {
            get { return Status == CommentStatus.Spam; }
            set { Status = value ? CommentStatus.Spam : CommentStatus.NotSpam; }
        }

        [Required]
        public virtual int RevisionNumber { get; set; }
    }
}
