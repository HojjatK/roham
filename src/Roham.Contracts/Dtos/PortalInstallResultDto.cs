using System;
using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class PortalInstallResultDto
    {
        public bool Succeed { get; set; }
        public List<string> UpgradeOutput { get; set; }
        public Exception Error { get; set; }
    }
}
