using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;

namespace Roham.Contracts.Commands.Category
{
    public class AddCategoryCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public long? ParentId { get; set; }

        public long SiteId { get; set; }

        public override string ToString()
        {
            return $"CategoryName:{Name}, Description:{Description}";
        }
    }
}
