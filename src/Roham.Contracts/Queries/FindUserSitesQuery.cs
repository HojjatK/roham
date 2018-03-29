using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindUserSitesQuery : IQuery<UserSitesAndZonesDto>
    {
        public string QueryString => $"username:{UserName}";

        // This can be null (for anounymous user)
        public string UserName { get; set; }

        public override string ToString()
        {
            return $"Find UserSites [{QueryString}]";
        }
    }
}
