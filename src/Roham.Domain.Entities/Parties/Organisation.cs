using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.DataAnnotation;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Parties
{
    /// <summary>
    /// Organisation domain entity.
    /// </summary>
    public class Organisation : Party
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_Organisation_Name")]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }
    }
}
