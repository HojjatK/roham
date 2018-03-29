using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Zone
{
    public class DeleteZoneCommand : AbstractCommand
    {
        public long ZoneId { get; set; }

        public override string ToString()
        {
            return $@"ZoneId: {ZoneId}";
        }
    }
}
