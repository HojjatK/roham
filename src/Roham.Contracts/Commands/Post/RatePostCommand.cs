using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class RatePostCommand : AbstractCommand
    {
        public long PostId { get; set; }

        public decimal Rating { get; set; }
    }
}
