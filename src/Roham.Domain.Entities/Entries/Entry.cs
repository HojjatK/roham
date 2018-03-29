using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Entries
{
    /// <summary>
    /// Abstract entry domain entity.
    /// </summary>
    public abstract class Entry : AggregateRoot                
    {
        protected Entry()
        {   
            Created = DateTime.UtcNow;            
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Title can not be null or empty")]
        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string MimeType { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Author { get; set; }

        [Required]
        public virtual bool IsPrivate { get; set; }

        [Required]
        public virtual bool IsContentBinary { get; set; }

        [Required]
        public virtual DateTime Created { get; set; }

        [Required]
        public virtual User Creator { get; set; }
    }

    /// <summary>
    /// Abstract generic entry domain entity.
    /// </summary>
    public abstract class Entry<TRevision, TComment, TRating, TLink> : Entry
        where TRevision : EntryRevision, new()
        where TComment: EntryComment, new()
        where TRating: EntryRating, new()
        where TLink: EntryLink
    {
        [Required]
        public virtual int CommentsCount { get; set; }

        [Required]
        public virtual decimal Rating { get; set; }

        public virtual int? DisableDiscussionDays { get; set; }

        [Required]
        public virtual bool IsDiscussionEnabled { get; set; }

        [Required]
        public virtual bool IsRatingEnabled { get; set; }

        [Required]
        public virtual bool IsAnonymousCommentAllowed { get; set; }

        public virtual TRevision LatestRevision { get; protected set; }

        private ICollection<TRevision> _revisions;
        public virtual ICollection<TRevision> Revisions
        {
            get { return this.LazySet(ref _revisions); }
            protected set { _revisions = value.AsSet(); }
        }

        private ICollection<TComment> _comments;
        public virtual ICollection<TComment> Comments
        {
            get { return this.LazySet(ref _comments); }
            protected set { _comments = value.AsSet(); }
        }

        private ICollection<TRating> _ratings;
        public virtual ICollection<TRating> Ratings
        {
            get { return this.LazySet(ref _ratings); }
            protected set { _ratings = value.AsSet(); }
        }

        private ICollection<TLink> _links;
        public virtual ICollection<TLink> Links
        {
            get { return this.LazySet(ref _links); }
            protected set { _links = value.AsSet(); }
        }

        public virtual TRevision Revise()
        {
            var original = LatestRevision;
            var revision = new TRevision();
            if (original != null)
            {
                revision.Body = original.Body;                
                revision.ReviseReason = original.ReviseReason;
            }            
            revision.RevisedDate = DateTime.UtcNow;            
            revision.RevisionNumber = Revisions.Count + 1;
            LatestRevision = revision;
            Revisions.Add(revision);
            return revision;
        }

        public virtual TComment Comment()
        {
            var comment = new TComment
            {   
                Posted = DateTime.UtcNow
            };
            Comments.Add(comment);
            CommentsCount = Comments.Count;
            return comment;
        }

        public virtual TRating Rate()
        {
            var rating = new TRating
            {   
                Rate = 0,
                RatedDate = DateTime.Now,
            };
            Ratings.Add(rating);
            // TODO: formula to update rate
            return rating;
        }
    }
}
