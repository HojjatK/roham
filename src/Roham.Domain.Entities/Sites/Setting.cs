using Roham.Lib.Strings;
using Roham.Lib.Domain;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Entities.Sites
{
    /// <summary>
    /// Setting domain entity.    
    /// </summary>
    /// <remarksSetting is global when site property is null, otherwise it is at site level</remarks>
    public class Setting : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual PageName Section { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(Lengths.Name)]
        public virtual PageName Name { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public virtual string Description { get; set; }

        [MaxLength(Lengths.SettingValue)]
        public virtual string Value { get; set; }

        public virtual Site Site { get; set; }
    }
}
