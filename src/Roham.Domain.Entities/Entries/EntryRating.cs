using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Entries
{
    /// <summary>
    /// Abstract entry rating domain entity.
    /// </summary>
    public abstract class EntryRating : Identifiable
    {
        protected EntryRating()
        {
        }

        [Required]
        public virtual decimal Rate { get; set; }

        [Required]
        public virtual DateTime RatedDate { get; set; }

        [MaxLength(Lengths.Email)]
        public virtual string UserIdentity { get; set; }

        [MaxLength(Lengths.Email)]
        public virtual string UserEmail { get; set; }
    }
}
