using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class DeletePostCommand : AbstractCommand
    {
        public long PostId { get; set; }
    }
}
