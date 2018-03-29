/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Entities.Jobs
{
    public enum TaskStatus
    {   
        InProgress = 1,
        Failed = 2,
        Succeed = 3
    }

    public class JobTask : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        [Required]
        public virtual TaskStatus Status { get; set; }

        [Required]
        public virtual int ProgressEstimate { get; set; }

        [Required]
        public virtual DateTime Created { get; set; }

        public virtual DateTime? Completed { get; set; }

        [MaxLength(Lengths.LogMessage)]
        public virtual string FailedMessage { get; set; }

        [MaxLength(Lengths.Email)]
        public virtual string OwnerUserName { get; set; }

        [Required]
        public virtual Job Job { get; set; }

        private ICollection<JobTaskDetail> _details;
        public virtual ICollection<JobTaskDetail> Details
        {
            get { return this.LazySet(ref _details); }
            protected set { _details = value.AsSet(); }
        }
    }
}