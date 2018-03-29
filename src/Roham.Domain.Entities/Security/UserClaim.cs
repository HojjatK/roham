using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Security
{
    /// <summary>
    /// User claim domain entity.
    /// </summary>
    public class UserClaim : Identifiable
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string ClaimType { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.LongDescription)]
        public virtual string ClaimValue { get; set; }

        [Required]
        public virtual User User { get; set; }
    }
}
