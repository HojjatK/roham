using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Strings;

namespace Roham.Contracts.Commands.EntrySerie
{
    public class AddEntrySerieCommand : AbstractCommand
    {
        [Required]
        public PageName Name { get; set; }
    }
}
