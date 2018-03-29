using Roham.Lib.Domain.Cache;

namespace Roham.Contracts.Dtos
{
    public class AppFunctionDto : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<AppFunctionDto, string>(nameof(Uid), Uid);
        
        public string Uid { get; set; }

        public long Id { get; set; }        

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool IsAllowed { get; set; }

        public AppFunctionDto Parent { get; set; }
    }
}
