using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindRoleFunctionsQuery : IQuery<RoleFunctionsDto>
    {
        public string QueryString => $"roleid:{RoleId}";

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        public override string ToString()
        {
            return $"Find RoleFunction [{QueryString}]";
        }
    }
}
