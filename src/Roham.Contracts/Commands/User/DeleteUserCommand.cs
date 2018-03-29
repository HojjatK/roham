using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class DeleteUserCommand : AbstractCommand
    {
        public long Id { get; set; }

        public override string ToString()
        {
            return $"UserId: {Id}";
        }
    }
}
