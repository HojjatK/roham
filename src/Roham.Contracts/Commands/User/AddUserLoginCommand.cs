using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class AddUserLoginCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LoginProvider { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ProviderKey { get; set; }

        public override string ToString()
        {
            return $@"
UserId:   {UserId},
Provider: {LoginProvider}"; // don't expose providerkey
        }
    }
}
