using Roham.Lib.Domain;
using Roham.Lib.Domain.Cache;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class RoleDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<RoleDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }

        public bool IsSystemRole { get; set; }

        [MaxLength(Lengths.Name)]
        public string RoleType { get; set; }
    }
}
