using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;

namespace Roham.Domain.Entities.Security
{
    /// <summary>
    /// User login domain entity.
    /// </summary>
    public class UserLogin : Identifiable
    {
        [Required(AllowEmptyStrings = false)]        
        [MaxLength(Lengths.LongName)]
        [Unique("UQ_UserLogin")]
        public virtual string LoginProvider { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Description)]
        [Unique("UQ_UserLogin")]
        public virtual string ProviderKey { get; set; }

        [Required]
        [Unique("UQ_UserLogin")]
        public virtual User User { get; set; }
    }
}
