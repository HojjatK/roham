using System;
using System.Collections.Generic;
using System.Linq;
using Roham.Lib.Strings;

namespace Roham.Contracts.Dtos
{
    public class UserSitesAndZonesDto
    {
        private List<UserSiteDto> _sites;
        public List<UserSiteDto> Sites
        {
            get
            {
                if (_sites == null)
                {
                    _sites = new List<UserSiteDto>();
                }
                return _sites;
            }
            set
            {
                _sites = value;
            }
        }

        public bool ContainsSite(string site)
        {
            return Sites.Any(s => string.Equals(s.Name, site, StringComparison.OrdinalIgnoreCase));
        }

        public bool ContainsZone(string site, string zone)
        {
            return Sites.Any(s => string.Equals(s.Name, site, StringComparison.OrdinalIgnoreCase) &&
                s.Zones.Any(z => string.Equals(z.Name, zone, StringComparison.OrdinalIgnoreCase)));
        }

        public class UserSiteDto
        {
            public PageName Name { get; set; }
            public bool IsDefault { get; set; }
            public bool IsActive { get; set; }
            public bool IsPublic { get; set; }

            private List<UserZoneDto> _zones;
            public List<UserZoneDto> Zones
            {
                get
                {
                    if (_zones == null)
                    {
                        _zones = new List<UserZoneDto>();
                    }
                    return _zones;
                }
                set { _zones = value; }
            }
        }

        public class UserZoneDto
        {
            public PageName Name { get; set; }
            public string Type { get; set; }
            public bool IsActive { get; set; }
            public bool IsPublic { get; set; }
        }
    }
}
