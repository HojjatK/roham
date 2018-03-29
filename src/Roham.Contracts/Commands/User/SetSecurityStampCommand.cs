using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class SetSecurityStampCommand : AbstractCommand
    {   
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SecurityStamp { get; set; }

        public override string ToString()
        {
            return $@"UserId: {UserId}"; // don't expose security stamp
        }
    }
}
