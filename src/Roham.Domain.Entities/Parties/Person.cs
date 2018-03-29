using Roham.Lib.Domain;
using System.ComponentModel.DataAnnotations;

namespace Roham.Domain.Entities.Parties
{
    /// <summary>
    /// Person domain entity.
    /// </summary>
    public class Person : Party
    {
        [MaxLength(Lengths.Title)]
        public virtual string Title { get; set; }

        [MaxLength(Lengths.Name)]
        [Required(AllowEmptyStrings = false)]
        public virtual string GivenName { get; set; }

        [MaxLength(Lengths.Name)]
        public virtual string MiddleName { get; set; }

        [MaxLength(Lengths.Name)]
        [Required(AllowEmptyStrings = false)]
        public virtual string Surname { get; set; }

        public virtual string GetFullName()
        {
            return string.Join(" ", new[] { Title, GivenName, MiddleName, Surname}).Trim();
        }
    }
}
