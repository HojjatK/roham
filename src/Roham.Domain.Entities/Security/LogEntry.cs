using System;
using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities.Security
{
    public enum LogEntryLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        FatalError
    }

    /// <summary>
    /// Log entry domain model
    /// </summary>
    public class LogEntry : AggregateRoot
    {
        [Required]
        public virtual DateTime Timestamp { get; set; }

        [Required]
        public virtual LogEntryLevel Level { get; set; }

        [Required]
        [MaxLength(Lengths.LogMessage)]
        public virtual string Message { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string LoggerName { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string Thread { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string ProcessUser { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string SessionToken { get; set; }

        [MaxLength(Lengths.LongName)]
        public virtual string SessionUser { get; set; }

        [MaxLength(Lengths.LogMessage)]
        public virtual string StackTrace { get; set; }

        [MaxLength(Lengths.LogMessage)]
        public virtual string ClientStackTrace { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public virtual string Exception { get; set; }

        [MaxLength(Lengths.LongDescription)]
        public virtual string Extra { get; set; }
    }
}
