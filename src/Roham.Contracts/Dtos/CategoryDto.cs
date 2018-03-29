/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.Cache;
using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class CategoryDto : CachableDto
    {
        public override CacheKey CacheKey => CacheKey.New<CategoryDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        public long? ParentId { get; set; }

        public string ParentName { get; set; }

        public long SiteId { get; set; }

        public string SiteTitle { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public string Description { get; set; }
    }

    public class CategoryNodeDto : CategoryDto
    {   
        public List<CategoryNodeDto> Children { get; set; }
    }
}