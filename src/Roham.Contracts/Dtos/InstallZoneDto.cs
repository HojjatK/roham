using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class InstallZoneDto 
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
