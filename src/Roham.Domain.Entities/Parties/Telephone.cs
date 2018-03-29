using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Parties
{
    public enum TelephoneTypes
    {
        Mobile,
        Home,
        Business,
        Fax
    }

    /// <summary>
    /// Telephone domain entity.
    /// </summary>
    public class Telephone : Identifiable
    {
        [Required]
        public virtual TelephoneTypes Type { get; set; }

        [MaxLength(Lengths.Number)]
        public virtual string Area { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.LongNumber)]
        public virtual string Number { get; set; }

        [Required]
        public virtual Party Party { get; set; }        
    }
}
