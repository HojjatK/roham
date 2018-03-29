using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Parties
{
    public enum AddressTypes
    {
        Residential = 1,
        NonResidential = 2,
    }

    /// <summary>
    /// Address domain entity
    /// </summary>
    public class Address : Identifiable
    {
        [Required]
        public virtual AddressTypes AddressType { get; set; }

        [MaxLength(Lengths.Name)]
        [Required(AllowEmptyStrings = false)]
        public virtual string AddressLine1 { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string AddressLine2 { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string AddressLine3 { get; set; }

        [MaxLength(Lengths.ShortName)]
        public virtual string PostCode { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Suburb { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string State { get; set; }

        [MaxLength(Lengths.Name)]
        [Required(AllowEmptyStrings = false)]
        public virtual string Country { get; set; }

        [Required]
        public virtual Party Party { get; set; }
    }
}
