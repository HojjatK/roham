using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Command;
using System.Collections.Generic;

namespace Roham.Contracts.Commands.User
{
    public class UpdateUserCommand : AbstractCommand
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string GivenName { get; set; }
                        
        public string MiddleName { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public List<long> RoleIds { get; set; }

        public List<long> SiteIds { get; set; }

        public bool IsIndividual => !string.IsNullOrWhiteSpace(GivenName) || !string.IsNullOrWhiteSpace(MiddleName) || !string.IsNullOrWhiteSpace(Surname);

        public override string ToString()
        {
            return $@"
UserId:   {Id}, 
FullName: {string.Join(" ", Title, GivenName, MiddleName, Surname).Trim()}";
        }
    }
}
