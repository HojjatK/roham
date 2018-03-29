using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;

namespace Roham.Contracts.Commands.Post
{
    public class SavePostCommand : AbstractCommand
    {
        public long SiteId { get; set; }

        public long ZoneId { get; set; }

        public long PostId { get; set; }

        public long SerieId { get; set; }

        [Required]
        public PageName Name { get; set; }

        [Required]
        public string Title { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string PageTemplate { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsDiscussionEnabled { get; set; }

        public int? DisableDiscussionDays { get; set; }

        public bool IsAnonymousCommentAllowed { get; set; }

        public bool IsRatingEnabled { get; set; }

        [Required]
        public string UserName { get; set; }

        public string TagsCommaSeparated { get; set; }

        public string ContentSummary { get; set; }

        [Required]
        public string Content { get; set; }

        public List<KeyValuePair<string, string>> Links { get; set; }
    }
}
