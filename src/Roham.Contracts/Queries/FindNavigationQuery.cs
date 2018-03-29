using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindNavigationQuery : IQuery<NavigationDto>
    {
        public string QueryString => $"username:{UserName}";

        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        public override string ToString()
        {
            return $"Find Navigation [{QueryString}]";
        }
    }
}
