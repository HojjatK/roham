using System.Collections.Generic;

namespace Roham.Contracts.Dtos
{
    public class RoleFunctionsDto : RoleDto
    {
        public List<AppFunctionDto> Functions { get; set; }

        public void Fill(RoleDto roleDto)
        {
            this.Uid = roleDto.Uid;
            this.Id = roleDto.Id;
            this.Name = roleDto.Name;
            this.Description = roleDto.Description;
            this.IsSystemRole = roleDto.IsSystemRole;
            this.RoleType = roleDto.RoleType;
        }
    }
}
