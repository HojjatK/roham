using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.User
{
    public class DeleteUserClaimCommand : AbstractCommand
    {
        public long UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClaimType { get; set; }

        public override string ToString()
        {
            return $@"
UserId:    {UserId}, 
ClaimType: {ClaimType}";
        }
    }
}
