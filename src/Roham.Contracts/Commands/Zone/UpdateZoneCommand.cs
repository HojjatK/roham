using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;

namespace Roham.Contracts.Commands.Zone
{
    public class UpdateZoneCommand : AbstractCommand
    {
        public long SiteId { get; set; }

        public long ZoneId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        public PageName Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $@"
SiteId:      {SiteId},
ZoneId:      {ZoneId}, 
Title:       {Title}, 
Name:        {Name}, 
IsActive:    {IsActive}, 
IsPublic:    {IsPublic}, 
Description: {Description}";
        }
    }
}
