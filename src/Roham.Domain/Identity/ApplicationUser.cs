using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Identity
{
    public class ApplicationUser : IUser<long>
    {
        public ApplicationUser(UserDto user)
        {
            Details = user;
        }

        public UserDto Details { get; private set; }

        public long Id
        {
            get { return Details.Id; }
        }

        public string UserName
        {
            get
            {
                return Details.UserName;
            }
            set
            {
                Details.UserName = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, long> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ClaimsIdentity GenerateUserIdentity(UserManager<ApplicationUser, long> manager)
        {
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}