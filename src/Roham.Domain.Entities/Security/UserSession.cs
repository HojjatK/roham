using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Security
{
    public enum SessionStatus
    {
        Active,
        Expired
    }

    /// <summary>
    /// User session domain entity.
    /// </summary>
    public class UserSession : AggregateRoot
    {   
        public virtual Guid Sid { get { return Uid; } }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual SessionStatus Status { get; set; }

        [Required]
        public virtual DateTime StartTimestamp { get; set; }

        public virtual DateTime? EndTimestamp { get; set; }
    }
}