using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class SetUserEmailConfirmCommand : AbstractCommand
    {
        public long UserId { get; set; }
        public bool Confirmed { get; set; }

        public override string ToString()
        {
            return $@"
UserId:    {UserId}, 
Confirmed: {Confirmed}";
        }
    }
}
