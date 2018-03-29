using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class DeletePostTagCommand : AbstractCommand
    {
        public long PostId { get; set; }
        public long TagId { get; set; }
    }
}
