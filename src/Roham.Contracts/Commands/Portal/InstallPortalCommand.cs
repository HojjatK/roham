using System.Linq;
using Roham.Lib.Domain.CQS.Command;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Contracts.Dtos;
using Roham.Lib.Emails;
using Roham.Lib.Domain;

namespace Roham.Contracts.Commands.Portal
{
    public class InstallPortalCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Name)]
        public string DatabaseProviderName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Connection)]
        public string ConnectionString { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.LongName)]
        public string CacheProviderName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(Lengths.Connection)]
        public string CacheConnectionString { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Name)]
        public string PortalName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [StringLength(Lengths.Name)]
        public string AdminUserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Password)]
        public string AdminPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Name)]
        public string SiteName { get; set; }

        [Required]
        public List<InstallZoneDto> SiteZones { get; set; }

        public bool UseSmtp { get; set; }

        [StringLength(Lengths.Email)]
        public string SmtpFrom { get; set; }

        public SmtpSettings SmtpSettings { get; set; }

        public List<string> UpgradeOutput { get; set; }

        public override string ToString()
        {
            return $@"
DbProvider: {DatabaseProviderName}, 
PortalName: {PortalName}, 
AdminUser:  {AdminUserName}, 
SiteName:   {SiteName}, 
Zones:      {string.Join(" ", SiteZones.Select(sz => sz.Code))}";
        }
    }
}
