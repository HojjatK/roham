using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Command;
using System;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Job
{
    public class AddJobCommand : AbstractCommand
    {
        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsSystemJob { get; set; }

        [MaxLength(Lengths.Description)]
        public string Description { get; set; }

        public DateTime Created { get; set; }

        public long? OwnerUserId { get; set; }

        public long? SiteId { get; set; }
    }
}
