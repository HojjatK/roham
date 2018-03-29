using System.Collections.Generic;
using System.Linq;

namespace Roham.Domain
{
    public static class BlackNameList
    {
        private static readonly ICollection<string> bannedNames = new HashSet<string>
        {
            "portal",
            "site",
            "zone",
            "entry",

            "admin",
            "administrator",
            "home",
            "install",
            "setup",
            "logoff",
            "logout",
            "login",
            "upgrade",
            "resource",
            "meta",
            "metaweblog",
            "meta-weblog",
            "tag",
            "tagged",
            "sql",
            "upload",
            "download",
            "error",

            "index",
            "page",
            "post",
            "recent",
            "recent-posts",
            "revisions",
            "revision",
            "unpublished",
            "published",
            "publish",
            "send",
            "receive",
            "auth",
            "authentication",
            "authorisation",
            "authorization",

            "blog",
            "article",
            "wiki",
            "news",

            "search",
            "feeds",
            "feed",
            "commentfeed",
            "robots",
            "favicon",
            "status",
            "sitemap",

            "content",
            "image",
            "images",
            "css",
            "views",
            "scripts",
            "controllers",
            "bin",
            "debug",
            "release",
            "font",
            "fonts",
            "log",
            "logs",
            "web",
            "app",

            "get",
            "post",
            "view",
            "new",
            "create",
            "edit",
            "update",
            "delete",
            "revert",
            "history",
            "history-of",
            "pingback",
            "pingbacks",
            "trackback",
            "trackbacks",
            "trackbacks-for",
            "rsd",
            "api"
        };

        private static readonly ICollection<string> bannedSiteNames = new HashSet<string>();
        private static readonly ICollection<string> bannedZoneNames = new HashSet<string>();
        private static readonly ICollection<string> bannedPageNames = new HashSet<string>();

        static BlackNameList()
        {
            bannedSiteNames.AddRange(bannedNames);
            bannedZoneNames.AddRange(bannedNames.Except(new List<string> { "blog", "article", "wiki", "news" }));
            bannedPageNames.AddRange(bannedNames);
        }

        public static ICollection<string> SiteNames { get { return bannedSiteNames; } }
        public static ICollection<string> ZoneNames { get { return bannedZoneNames; } }
        public static ICollection<string> PageNames { get { return bannedPageNames; } }
    }
}
