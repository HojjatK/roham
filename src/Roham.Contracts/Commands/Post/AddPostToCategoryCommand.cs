using Roham.Lib.Domain.CQS.Command;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Post
{
    public class AddPostToCategoryCommand : AbstractCommand
    {
        public long PostId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
