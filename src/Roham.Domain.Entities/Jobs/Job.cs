using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;
using Roham.Domain.Entities.Sites;

namespace Roham.Domain.Entities.Jobs
{
    public enum JobType
    {
        ImportEntries,
        ExportEntries,
    }

    public class Job : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        public virtual JobType Type { get; set;}

        public virtual bool IsSystemJob { get; set; }

        [MaxLength(Lengths.Description)]
        public virtual string Description { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public virtual string CronPattern { get; set; }

        [Required]
        public virtual DateTime Created { get; set; }

        [Required]
        public virtual User Owner { get; set; }

        public virtual Site Site { get; set; }

        private ICollection<JobTask> _tasks;
        public virtual ICollection<JobTask> Tasks
        {
            get { return this.LazySet(ref _tasks); }
            protected set { _tasks = value.AsSet(); }
        }
    }

    //public enum JobStatus
    //{
    //    Scheduled = 1,
    //    Running = 2,
    //    Failed = 3,
    //    Success = 4
    //} 

    ///// <summary>
    ///// Job domain entity.
    ///// </summary>
    //public class Job : AggregateRoot
    //{
    //    [Required(AllowEmptyStrings = false)]
    //    [MaxLength(Lengths.Name)]
    //    public virtual string Name { get; set; }

    //    [MaxLength(Lengths.LongDescription)]
    //    public virtual string Arguments { get; set; }

    //    [Required]
    //    public virtual JobStatus Status { get; set; }

    //    [Required]
    //    public virtual int ProgressEstimate { get; set; }

    //    [Required]
    //    public virtual bool IsSystemJob { get; set; }

    //    [Required]
    //    public virtual DateTime Created { get; set; }

    //    public virtual DateTime? Updated { get; set; }

    //    [MaxLength(Lengths.LogMessage)]
    //    public virtual string FailedMessage { get; set; }

    //    [Required]
    //    public virtual User Owner { get; set; }

    //    public virtual Site Site { get; set; }

    //    private ICollection<JobDetail> _details;
    //    public virtual ICollection<JobDetail> Details
    //    {
    //        get { return this.LazySet(ref _details); }
    //        protected set { _details = value.AsSet(); }
    //    }
    //}
}
