/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;
using System;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class JobDto : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<JobDto, string>(nameof(Uid), Uid);

        public long Id { get; set; }

        public string Uid { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Type { get; set; }

        [MaxLength(Lengths.Description)]
        public string TypeDescription { get; set; }

        public bool IsSystemJob { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }

        public DateTime Created { get; set; }

        public long? OwnerUserId { get; set; }

        [MaxLength(Lengths.Email)]
        public string OwnerUser { get; set; }

        public long? SiteId { get; set; }

        public string SiteTitle { get; set; }
    }
}