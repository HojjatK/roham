using System.ComponentModel.DataAnnotations;
using Roham.Lib.Strings;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.EntrySerie
{
    public class UpdateEntrySerieCommand : AbstractCommand
    {
        public long SerieId { get; set; }

        [Required]
        public PageName Name { get; set; }

        public override string ToString()
        {
            return $"SerieId:{SerieId}, Name:{Name}";
        }
    }
}
