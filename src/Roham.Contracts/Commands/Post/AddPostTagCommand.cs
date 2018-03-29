using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Post
{
    public class AddPostTagCommand : AbstractCommand
    {
        public long PostId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
