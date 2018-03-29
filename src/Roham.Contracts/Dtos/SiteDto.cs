using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class SiteDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<SiteDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        public bool IsPublic { get; set; }

        [MaxLength(Lengths.Email)]
        public string SiteOwner { get; set; }

        public List<ZoneDto> Zones { get; set; }
    }
}
