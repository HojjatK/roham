using Roham.Contracts.Dtos;
using Roham.Lib.Domain.CQS.Command;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Role
{
    public class UpdateRoleCommand : AbstractCommand
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IncludeFuctions { get; set; }

        public List<AppFunctionDto> Functions { get; set; }

        public override string ToString()
        {
            return $"RoleId:{Id}, Name:{Name}, Description:{Description}";
        }
    }
}
