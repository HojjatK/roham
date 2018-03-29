/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using Roham.Lib.Strings;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{   
    public class FindPostByNameQuery : IQuery<PostDto>
    {
        public FindPostByNameQuery(string siteName, string zoneName, string postName)
        {
            SiteName = siteName;
            ZoneName = zoneName;
            PostName = postName;
        }

        public PageName SiteName { get; }

        public PageName ZoneName { get; }

        public PageName PostName { get; }

        public string Uri
        {
            get
            {
                return $"{SiteName}/{ZoneName}/{PostName}";
            }
        }

        public string QueryString
        {
            get
            {
                return $"site={SiteName}&zone={ZoneName}&post={PostName}";
            }
        }
    }
}