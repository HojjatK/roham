using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class PostDto : PostItemDto<PostDto>
    {   
        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string PageTemplate { get; set; }

        public bool IsDiscussionEnabled { get; set; }

        public int? DisableDiscussionDays { get; set; }

        public bool IsRatingEnabled { get; set; }

        public bool IsAnonymousCommentAllowed { get; set; }

        public string CategoriesCommaSeperated { get; set; }

        public string ConentSummary { get; set; }

        public string Content { get; set; }

        public List<CommentDto> Comments { get; set; }

        public List<LinkDto> Links { get; set; }
        
    }
}
