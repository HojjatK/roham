using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindZonesBySiteIdQuery : IQuery<List<ZoneDto>>
    {
        public FindZonesBySiteIdQuery(long siteId)
        {
            SiteId = siteId;
        }

        public string QueryString => $"siteid:{SiteId}";

        [Range(1, long.MaxValue)]
        public long SiteId { get; private set; }

        public override string ToString()
        {
            return $"Find Zones [{QueryString}]";
        }
    }
}
