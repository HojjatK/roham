using Microsoft.AspNet.Identity;

namespace Roham.Domain.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser, long>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, long> store)
          : base(store)
        {
        }
    }
}
