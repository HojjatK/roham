using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Role
{
    public class AddRoleCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RoleType { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, Description:{Description}, RoleType:{RoleType}";
        }
    }
}
