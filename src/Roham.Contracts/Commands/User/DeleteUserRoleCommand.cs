using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class DeleteUserRoleCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string RoleName { get; set; }

        public override string ToString()
        {
            return $@"
UserId:   {UserId}, 
RoleName: {RoleName}";
        }
    }
}
