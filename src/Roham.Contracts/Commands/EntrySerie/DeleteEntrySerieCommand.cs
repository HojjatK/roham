using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.EntrySerie
{
    public class DeleteEntrySerieCommand : AbstractCommand
    {
        public long SerieId { get; set; }
    }
}
