using System;
using System.Linq;
using System.Collections.Generic;
using Roham.Lib.Ioc;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class RoleMapper : IEntityMapper<RoleDto, Role>
    {
        public RoleDto Map(Role role)
        {
            return new RoleDto
            {
                Uid = role.Uid.ToString(),
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                RoleType = role.RoleType.ToString(),
            };
        }

        public static List<KeyValuePair<string, string>> GetAllRoleTypes()
        {   
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(RoleTypeCodes.SystemAdmin.ToString(), "System Administrator"),
                new KeyValuePair<string, string>(RoleTypeCodes.Administrator.ToString(), "Aministrator" ),
                new KeyValuePair<string, string>(RoleTypeCodes.User.ToString(), "User" ),
            };
        }

        public static List<KeyValuePair<string, string>> GetAvaliableRoleTypes()
        {
            return GetAllRoleTypes()
                .Where(item => !RoleTypeCodes.SystemAdmin.ToString().Equals(item.Key, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
