/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;

namespace Roham.Contracts.Dtos
{
    public class TaskDto : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<TaskDto, string>(nameof(Uid), Uid);

        public long Id { get; set; }

        public string Uid { get; set; }

        public long JobId { get; set; }

        public string JobName { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Status { get; set; }

        public int ProgressEstimate { get; set; }

        public DateTime Created { get; set; }

        public virtual DateTime? Completed { get; set; }

        [MaxLength(Lengths.LogMessage)]
        public virtual string FailedMessage { get; set; }

        [MaxLength(Lengths.Email)]
        public virtual string OwnerUserName { get; set; }

        public List<TaskDetailDto> Details { get; set; }
    }
}