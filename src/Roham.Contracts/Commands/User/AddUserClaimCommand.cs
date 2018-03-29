using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class AddUserClaimCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClaimValueType { get; set; }

        public override string ToString()
        {
            return $@"
UserId:         {UserId},
ClaimType:      {ClaimType},
ClaimValue:     {ClaimValue},
ClaimValueType: {ClaimValueType}";
        }
    }
}
