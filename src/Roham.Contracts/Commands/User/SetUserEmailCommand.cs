using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class SetUserEmailCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        public override string ToString()
        {
            return $@"
UserId: {UserId}, 
Email:  {Email}";
        }
    }
}
