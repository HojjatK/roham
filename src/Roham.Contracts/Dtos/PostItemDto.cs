/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Domain.Cache;
using Roham.Lib.Strings;
using System;

namespace Roham.Contracts.Dtos
{
    public abstract class PostItemDto<T> : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<T, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        public int RevisionNumber { get; set; }

        public long SiteId { get; set; }

        public string SiteName { get; set; }

        public string SiteTitle { get; set; }

        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public string ZoneTitle { get; set; }

        public long SerieId { get; set; }

        public string SerieTitle { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string Author { get; set; }

        public bool IsPrivate { get; set; }

        public int CommentsCount { get; set; }

        public decimal Rating { get; set; }

        public string WorkflowStatus { get; set; }

        public string Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Published { get; set; }

        public DateTime? DisplayDate { get; set; }

        public string TagsCommaSeparated { get; set; }
    }

    public class PostItemDto : PostItemDto<PostItemDto>
    {
    }
}