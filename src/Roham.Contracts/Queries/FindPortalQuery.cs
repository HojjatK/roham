using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindPortalQuery : IQuery<PortalDto>
    {
        public string QueryString => "";

        public override string ToString()
        {
            return "Find Portal";
        }
    }
}
