/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.CQS.Query;
using System.Collections.Generic;

namespace Roham.Contracts.Queries
{
    public class FindZonesByNameQuery : IQuery<List<ZoneDto>>
    {
        public FindZonesByNameQuery(string zoneName, string sitename = null)
        {
            ZoneName = zoneName;
            SiteName = sitename;
        }

        public string SiteNameQueryString => string.IsNullOrWhiteSpace(SiteName) ? "" : $"& SiteName={SiteName}";

        public string QueryString => $"ZoneName={ZoneName}{SiteNameQueryString}";
        
        public string ZoneName { get; }
        public string SiteName { get;  }

        public override string ToString()
        {
            return $"Find Zones [{QueryString}]";
        }
    }
}