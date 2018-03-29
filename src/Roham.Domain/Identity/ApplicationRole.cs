using Microsoft.AspNet.Identity;
using Roham.Contracts.Dtos;
using Roham.Domain.Entities.Security;
using System;

namespace Roham.Domain.Identity
{
    public class ApplicationRole : IRole<long>
    {
        private string _name;

        public ApplicationRole(RoleDto role)
        {
            Details = role;

            var roleType = (RoleTypeCodes)Enum.Parse(typeof(RoleTypeCodes), role.RoleType);
            _name = SecurityRoleNames.GetSecurityRoleName(RoleTypeCodes.SystemAdmin);
            if (_name == null)
            {
                throw new NotSupportedException($"{roleType} role name is not supported");
            }
        }

        public RoleDto Details
        {
            get;
            private set;
        }

        public long Id
        {
            get { return Details.Id; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != SecurityRoleNames.SysAdmin &&
                    _name != SecurityRoleNames.Admin &&
                    _name != SecurityRoleNames.User)
                {
                    throw new NotSupportedException($"{value} role name is not supported");
                }
                _name = value;
            }
        }
    }
}
