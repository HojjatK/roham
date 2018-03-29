using Roham.Lib.Domain.CQS.Command;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Commands.User
{
    public class AddUserCommand : AbstractCommand
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        public bool IsSystemUser { get; set; }
        
        public string Title { get; set; }
        
        public string GivenName { get; set; }

        public string MiddleName { get; set; }
        
        public string Surname { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public List<long> RoleIds { get; set; }

        public List<long> SiteIds { get; set; }

        public bool IsIndividual => !string.IsNullOrWhiteSpace(GivenName) || !string.IsNullOrWhiteSpace(MiddleName)  || !string.IsNullOrWhiteSpace(Surname);

        public override string ToString()
        {
            return $@"
UserName:      {UserName}, 
FullName:      {string.Join(" ", Title, GivenName, Surname).Trim()}, 
Email:         {Email}, 
Phone:         {PhoneNumber}, 
IsSystemUser:  {IsSystemUser}";
        }
    }
}
