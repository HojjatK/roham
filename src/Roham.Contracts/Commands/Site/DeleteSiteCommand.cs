using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Site
{
    public class DeleteSiteCommand : AbstractCommand
    {
        public long Id { get; set; }

        public override string ToString()
        {
            return $"SiteId:{Id}";
        }
    }
}
