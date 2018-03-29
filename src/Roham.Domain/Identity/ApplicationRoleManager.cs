using Microsoft.AspNet.Identity;

namespace Roham.Domain.Identity
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole, long>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, long> roleStore)
            : base(roleStore)
        {
        }
    }
}
