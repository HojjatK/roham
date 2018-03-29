using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Command;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.Job
{
    public class ExecuteTaskCommand : AbstractCommand
    {
        public long JobId { get; set; }

        [Required]
        [MaxLength(Lengths.Name)]
        public string Name { get; set; }

        public string OwnerUserName { get; set; }
    }
}
