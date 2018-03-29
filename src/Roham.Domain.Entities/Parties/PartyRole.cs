using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Lib.Domain.DataAnnotation;

namespace Roham.Domain.Entities.Parties
{
    /// <summary>
    /// Party role domain entity.
    /// </summary>
    public class PartyRole : AggregateRoot
    {
        [Required(AllowEmptyStrings = false)]
        [Unique("UQ_PartyRole_Name")]
        [MaxLength(Lengths.Name)]
        public virtual string Name { get; set; }

        [MaxLength(Lengths.Description)]
        [Required(AllowEmptyStrings = false)]
        public virtual string Description { get; set; }

        // ManyToMany relation is controlled by Party entity, so keep Parties property as protected
        private ICollection<Party> _parties;
        protected virtual ICollection<Party> Parties
        {
            get { return this.LazySet(ref _parties); }
            set { _parties = value.AsSet(); }
        }

        public static string NameOfParties => nameof(Parties);
    }
}
