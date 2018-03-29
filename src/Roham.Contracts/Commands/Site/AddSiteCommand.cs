using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;

namespace Roham.Contracts.Commands.Site
{
    public class AddSiteCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        public string SiteTitle { get; set; }

        [Required(AllowEmptyStrings = false)]
        public PageName Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDefault { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string OwnerUsername { get; set; }

        public override string ToString()
        {
            return $@"
Title:         {SiteTitle},
Name:          {Name},
IsActive:      {IsActive},
IsPublic:      {IsPublic},
IsDefault:     {IsDefault},
OwnerUsername: {OwnerUsername}";
        }
    }
}
