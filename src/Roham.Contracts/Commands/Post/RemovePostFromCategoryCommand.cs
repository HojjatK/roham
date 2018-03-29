using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class RemovePostFromCategoryCommand : AbstractCommand
    {
        public long PostId { get; set; }

        public long CategoryId { get; set; }
    }
}
