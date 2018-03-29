using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Category
{
    public class UpdateCategoryCommand : AbstractCommand
    {
        public long CategoryId { get; set; }

        public long? ParentCategoryId { get; set; }

        public bool IsPublic { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $"CategoryId:{CategoryId}, Description:{Description}";
        }
    }
}
