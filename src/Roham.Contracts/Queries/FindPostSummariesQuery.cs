/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Strings;

namespace Roham.Contracts.Queries
{
    public class FindPostSummariesQuery : IQuery<List<PostSummaryDto>>
    {
        public FindPostSummariesQuery(string siteName, string zoneName)
        {
            SiteName = siteName;
            ZoneName = zoneName;
        }

        public PageName SiteName { get; }

        public PageName ZoneName { get; }

        public string Uri
        {
            get
            {
                return $"{SiteName}/{ZoneName}";
            }
        }

        public string QueryString
        {
            get
            {
                return $"site={SiteName}&zone={ZoneName}";
            }
        }
    }
}