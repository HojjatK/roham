using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class AddUserRoleCommand : AbstractCommand
    {
        public long UserId { get; set; }
        public string RoleName { get; set; }

        public override string ToString()
        {
            return $@"
UserId:    {UserId}, 
RoleName:  {RoleName}";
        }
    }
}
