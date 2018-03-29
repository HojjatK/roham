using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Site
{
    public class UpdateSiteCommand : AbstractCommand
    {
        public long SiteId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SiteTitle { get; set; }

        [Required(AllowEmptyStrings = false)]
        public PageName Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public override string ToString()
        {
            return $@"
SiteId:   {SiteId},
Title:    {SiteTitle},
Name:     {Name},
IsActive: {IsActive},
IsPublic: {IsPublic}";
        }
    }
}
