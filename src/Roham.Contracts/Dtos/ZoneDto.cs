using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class ZoneDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<ZoneDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        public long SiteId { get; set; }
                
        [MaxLength(Lengths.Name)]
        public string SiteTitle { get; set; }

        [MaxLength(Lengths.Name)]
        public string SiteName { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }

        [MaxLength(Lengths.Name)]
        public string ZoneType { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }
    }
}
