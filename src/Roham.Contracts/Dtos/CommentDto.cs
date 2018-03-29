using Roham.Lib.Domain.Cache;
using System;

namespace Roham.Contracts.Dtos
{
    public class CommentDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<CommentDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        public long postId { get; set; }

        public string postTitle { get; set; }

        public string AuthorName { get; set; }

        public string AuthorUrl { get; set; }

        public string AuthorEmail { get; set; }

        public string AuthorIp { get; set; }

        public string Body { get; set; }

        public DateTime Posted { get; set; }

        public string Status { get; set; }
    }
}
