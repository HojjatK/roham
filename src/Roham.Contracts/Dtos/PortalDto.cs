using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class PortalDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<PortalDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [MaxLength(Lengths.Name)]
        public string Title { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }
        
        [MaxLength(Lengths.LongDescription)]
        public string DatabaseInfo { get; set; }

        public PortalSettingsDto Settings { get; set; }
    }
}
