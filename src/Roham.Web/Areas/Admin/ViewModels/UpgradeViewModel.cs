using System.Collections.Generic;

namespace Roham.Web.Areas.Admin.ViewModels
{
    public class UpgradeViewModel
    {
        public string CacheKey { get; set; }

        public bool UpgradeRequried { get; set; }

        public List<string> Scripts { get; set; }

        public List<string> UpgradeOutput { get; set; }

        public string Message { get; set; }
    }
}
