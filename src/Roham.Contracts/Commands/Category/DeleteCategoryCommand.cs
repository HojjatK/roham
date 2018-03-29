using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Category
{
    public class DeleteCategoryCommand : AbstractCommand
    {
        public long CategoryId { get; set; }

        public override string ToString()
        {
            return $"CategoryId:{CategoryId}";
        }
    }
}
