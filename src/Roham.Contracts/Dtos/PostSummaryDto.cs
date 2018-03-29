using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class PostSummaryDto : PostItemDto<PostSummaryDto>
    {   
        public string ConentSummary { get; set; }

        public string Content { get; set; }

        public List<LinkDto> Links { get; set; }
    }
}
