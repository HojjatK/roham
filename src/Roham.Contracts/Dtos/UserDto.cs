using Roham.Lib.Domain.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class IdNamePair
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDto : CachableDto
    {   
        public override CacheKey CacheKey => CacheKey.New<UserDto, string>(nameof(Uid), Uid);

        public string Uid { get; set; }

        public long Id { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public bool EmailConfirm { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(150)]
        public string GivenName { get; set; }

        [MaxLength(150)]
        public string Surname { get; set; }

        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        [MaxLength(255)]
        public string SecurityStamp { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsSystemUser { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(255)]
        public string StatusReason { get; set; }

        [MaxLength(20)]
        public string PasswordHashAlgorithm { get; set; }

        [MaxLength(255)]
        public string PasswordHash { get; set; }

        public List<IdNamePair> SiteIdNames { get; set; }

        public List<IdNamePair> RoleIdNames { get; set; }

        public string FullName
        {
            get { return string.Join(" ", Title, GivenName, Surname).Trim(); }
        }
    }
}
