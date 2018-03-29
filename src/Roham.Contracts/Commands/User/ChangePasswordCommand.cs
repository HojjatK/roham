using Roham.Lib.Domain.CQS.Command;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.User
{
    public class ChangePasswordCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string NewPasswordHash { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PasswordHashAlgorithm { get; set; }

        public override string ToString()
        {
            return $@"
UserId:        {UserId}, 
HashAlgorithm: {PasswordHashAlgorithm}"; // don't expose password hashses
        }
    }
}
