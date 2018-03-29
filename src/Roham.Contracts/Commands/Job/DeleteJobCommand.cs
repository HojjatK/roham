using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Job
{
    public class DeleteJobCommand : AbstractCommand
    {
        public long JobId { get; set; }
    }
}
