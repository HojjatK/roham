using Roham.Contracts.Dtos;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Portal
{
    public class UpdatePortalCommand : AbstractCommand
    {   
        [Required]
        public PageName Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(Lengths.Name)]
        public string Title { get; set; }
        
        [StringLength(Lengths.Description)]
        public string Description { get; set; }

        public PortalSettingsDto SettingsDto { get; set; }

        public override string ToString()
        {
            return $"Name:{Name} Title:{Title}";
        }
    }
}
