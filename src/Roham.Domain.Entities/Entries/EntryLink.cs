using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Entries
{
    /// <summary>
    /// Abstract entry link domain entity.
    /// </summary>
    public abstract class EntryLink : Identifiable
    {
        [Required (AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual string Type { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.LongDescription)]
        public virtual string Ref { get; set; }
    }
}