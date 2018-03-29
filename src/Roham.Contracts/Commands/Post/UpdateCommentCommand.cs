using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class UpdateCommentCommand : AbstractCommand
    {
        public long PostId { get; set; }

        public long CommentId { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return $"PostId:{PostId}, CommentId:{CommentId}";
        }
    }
}
