using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Role
{
    public class DeleteRoleCommand : AbstractCommand
    {
        public long Id { get; set; }

        public override string ToString()
        {
            return $"RoleId:{Id}";
        }
    }
}
