using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Posts;

namespace Roham.Domain.Entities.Security
{
    /// <summary>
    /// Post entry's permission domain entity.
    /// </summary>
    public class PostPermission : Identifiable
    {
        public PostPermission()
        {
            Read = true;
        }

        [Required]
        public virtual bool Read { get; set; }

        [Required]
        public virtual bool Create { get; set; }

        [Required]
        public virtual bool Update { get; set; }

        [Required]
        public virtual bool Delete { get; set; }

        [Required]
        public virtual bool Execute { get; set; }

        [Required]
        public virtual User User{ get; set; }

        [Required]
        public virtual Post Post { get; set; }
    }
}
