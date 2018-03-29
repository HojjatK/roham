using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;
using Roham.Domain.Entities.Entries;

namespace Roham.Domain.Entities.Posts
{
    public enum ContentFormats
    {
        Html = 0,
        Markdown = 1
    }

    public enum PostStatus
    {   
        Saved = 0,
        Submitted = 1,
        Rejected = 2,
        Approved = 3,        
        Published = 4
    }

    /// <summary>
    /// Post entry domain model.
    /// </summary>
    public class Post : Entry<PostRevision, Comment, Rating, PostLink>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide a name.")]
        [Unique("UQ_Site_Post_Name")]
        [MaxLength(Lengths.Name)]
        public virtual PageName Name { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string MetaTitle { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public virtual string MetaDescription { get; set; }

        [Required]
        public virtual ContentFormats Format { get; set; }

        [Required]
        public virtual long ViewsCount { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string PageTemplate { get; set; }

        [Required]
        public virtual PostStatus Status { get; set; }

        public virtual DateTime? PublishDate { get; set; }

        public virtual DateTime? EffectiveDate { get; set; }

        [Required]
        public virtual decimal Popularity { get; set; }

        [Required]
        public virtual bool IsChromeHidden { get; set; }

        [Required]
        public virtual bool IsPingbackEnabled { get; set; }

        [Required]
        public virtual bool IsTrackbackEnabled { get; set; }        

        [Required]
        public virtual Zone Zone { get; set; }

        public virtual PostSerie Serie { get; set; }

        [Required]
        [Unique("UQ_Site_Post_Name")]
        public virtual Site Site { get; set; }

        private ICollection<PostPermission> _permissions;
        public virtual ICollection<PostPermission> Permissions
        {
            get { return this.LazySet(ref _permissions); }
            protected set { _permissions = value.AsSet(); }
        }

        public virtual ICollection<Category> Categories => Tags.Where(t => t.IsCategory).Cast<Category>().ToList();

        private ICollection<Tag> _tags;
        public virtual ICollection<Tag> Tags
        {
            get { return this.LazySet(ref _tags); }
            protected set { _tags = value.AsSet(); }
        }

        public virtual string TagsCommaSeparated
        {
            get { return string.Join(",", Tags.Select(t => t.Name)); }
            protected set { }
        }

        private ICollection<Pingback> _pingbacks;
        public virtual ICollection<Pingback> Pingbacks
        {
            get { return this.LazySet(ref _pingbacks); }
            protected set { _pingbacks = value.AsSet(); }
        }

        public override PostRevision Revise()
        {
            var revision = base.Revise();            
            revision.Post = this;            
            return revision;
        }

        public override Comment Comment()
        {
            var comment = base.Comment();
            comment.Post = this;
            return comment;            
        }

        public override Rating Rate()
        {
            var rating = base.Rate();
            rating.Post = this;
            return rating;
        }
    }
}
