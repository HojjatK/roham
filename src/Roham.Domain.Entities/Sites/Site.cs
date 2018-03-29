using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Strings;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Domain.Entities.Posts;
using Roham.Domain.Entities.Jobs;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Entities.Sites
{   
    /// <summary>
    /// Site domain entity.
    /// </summary>
    public class Site : AggregateRoot, INamed
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Site_Name")]
        [MaxLength(Lengths.Name)]
        public virtual PageName Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        [Required]
        public virtual bool IsDefault { get; set; }

        [Required]
        public virtual bool IsActive { get; set; }

        [Required]
        public virtual bool IsPrivate { get; set; }

        [Required]
        public virtual Portal Portal { get; set; }

        [Required]
        public virtual User Owner { get; set; }

        private ICollection<Setting> _siteSettings;
        public virtual ICollection<Setting> Settings
        {
            get { return this.LazySet(ref _siteSettings); }
            protected set { _siteSettings = value.AsSet(); }
        }

        // many-to-many
        private ICollection<User> _siteUsers;
        public virtual ICollection<User> Users
        {
            get { return this.LazySet(ref _siteUsers); }
            protected set { _siteUsers = value.AsSet(); }
        }

        private ICollection<PostWorkflowRule> _workflowRules;
        public virtual ICollection<PostWorkflowRule> WorkflowRules
        {
            get { return this.LazySet(ref _workflowRules); }
            protected set { _workflowRules = value.AsSet(); }
        }

        private ICollection<Zone> _zones;
        protected virtual ICollection<Zone> Zones
        {
            get { return this.LazySet(ref _zones); }
            set { _zones = value.AsSet(); }
        }

        private ICollection<Tag> _siteTags;
        protected virtual ICollection<Tag> Tags
        {
            get { return this.LazySet(ref _siteTags); }
            set { _siteTags = value.AsSet(); }
        }

        private ICollection<Job> _siteJobs;
        protected virtual ICollection<Job> Jobs
        {
            get { return this.LazySet(ref _siteJobs); }
            set { _siteJobs = value.AsSet(); }
        }

        private ICollection<PostSerie> _sitePostSeries;
        protected virtual ICollection<PostSerie> PostSeries
        {
            get { return this.LazySet(ref _sitePostSeries); }
            set { _sitePostSeries = value.AsSet(); }
        }

        private ICollection<Post> _siteEntries;
        protected virtual ICollection<Post> Entries
        {
            get { return this.LazySet(ref _siteEntries); }
            set { _siteEntries = value.AsSet(); }
        }

        public virtual string GetName() { return Name; }

        public static string NameOfZones => nameof(Zones);
        public static string NameOfTags => nameof(Tags);
        public static string NameOfJobs => nameof(Jobs);
        public static string NameOfPostSeries => nameof(PostSeries);
        public static string NameOfEntries => nameof(Entries);
    }
}
