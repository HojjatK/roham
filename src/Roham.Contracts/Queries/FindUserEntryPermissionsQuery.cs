using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindUserEntryPermissionsQuery :  IQuery<List<UserPostPermissionDto>>
    {
        public string QueryString => $"userid:{UserId}";

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        public override string ToString()
        {
            return $"Find UserEntryPermission [{QueryString}]";
        }
    }
}
