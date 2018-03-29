using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;

namespace Roham.Contracts.Commands.Zone
{
    public class AddZoneCommand : AbstractCommand
    {
        public long SiteId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        public PageName Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ZoneType { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $@"
SiteId:   {SiteId}, 
Title:    {Title}, 
Name:     {Name},
ZoneType: {ZoneType}, 
IsActive: {IsActive}, 
IsPublic: {IsPublic}";
        }
    }
}
