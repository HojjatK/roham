/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using System.Collections.Generic;
using Roham.Lib.Domain.Cache;


namespace Roham.Contracts.Dtos
{
    public class PostSerieDto : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<PostDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        public long? SiteId { get; set; }

        public string SiteTitle { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPrivate { get; set; }
    }
}