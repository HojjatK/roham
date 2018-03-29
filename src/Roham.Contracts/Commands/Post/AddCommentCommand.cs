using Roham.Lib.Domain.CQS.Command;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Post
{
    public class AddCommentCommand : AbstractCommand
    {
        public long PostId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"PostId:{PostId}";
        }
    }
}
