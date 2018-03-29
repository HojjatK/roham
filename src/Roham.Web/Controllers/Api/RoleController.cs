using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Domain.Mappers;
using Roham.Contracts.Commands.Role;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Security;

namespace Roham.Web.Controllers.Api
{   
    [RoutePrefix("api/role")]
    [Authorize(Roles = SecurityRoleNames.SysAdmin_Admin)]
    public class RoleController : ApiControllerBase
    {
        public RoleController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher) {}

        [HttpGet]
        [Route("type")]
        public List<KeyValuePair<string, string>> GetAvailableRoleTypes()
        {
            return RoleMapper.GetAvaliableRoleTypes();
        }

        [HttpGet]
        [Route("appfuncs")]
        public List<AppFunctionDto> GetAppFunctions()
        {
            return QueryExecutor.Execute(new FindAllQuery<AppFunctionDto, AppFunction>());
        }

        [HttpGet]
        [Route("functions")]
        public List<RoleFunctionsDto> GetRolesFunctions()
        {
            var results = new List<RoleFunctionsDto>();
            var roles = QueryExecutor.Execute(new FindAllQuery<RoleDto, Role>());
            foreach(var role in roles)
            {
                var roleFuncs = QueryExecutor.Execute(new FindRoleFunctionsQuery { RoleId = role.Id });
                results.Add(roleFuncs);
            }
            return results;
        }

        [HttpGet]
        [Route("{id:long}/functions")]
        public RoleFunctionsDto GetRoleFunctions(long id)
        {
            return QueryExecutor.Execute(new FindRoleFunctionsQuery { RoleId = id });
        }

        [HttpPut]        
        [Route("{id:long}/functions")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto UpdateRoleFunctions(long id, RoleFunctionsDto roleFunctions)
        {
            return Result(() =>
            {
                var command = new UpdateRoleCommand
                {
                    Id = id,
                    Name = roleFunctions.Name,
                    Description = roleFunctions.Description,
                    IncludeFuctions = roleFunctions.IsSystemRole ? false : true,
                    Functions = roleFunctions.Functions,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpGet]
        [Route("")]
        public List<RoleDto> GetRoles()
        {
            return QueryExecutor.Execute(new FindAllQuery<RoleDto, Role>());
        }

        [HttpGet]
        [Route("available")]
        public List<RoleDto> GetAvailableRoles()
        {
            // system admin role is not available for external use
            var allRoles = QueryExecutor.Execute(new FindAllQuery<RoleDto, Role>());
            return allRoles.Where(r => r.RoleType != RoleTypeCodes.SystemAdmin.ToString()).ToList();
        }

        [HttpGet]
        [Route("{id:long}")]
        public RoleDto GetRole(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<RoleDto, Role>(id));
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]        
        public ResultDto CreateRole(RoleDto newRole)
        {
            return Result(() =>
            {
                var command = new AddRoleCommand
                {
                    Name = newRole.Name,
                    RoleType = newRole.RoleType,
                    Description = newRole.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]        
        [Route("{id:long}")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto UpdateRole(long id, RoleDto role)
        {
            return Result(() =>
            {
                var command = new UpdateRoleCommand
                {
                    Id = id,
                    Name = role.Name,
                    Description = role.Description,
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]        
        [Route("{id:long}")]
        [Authorize(Roles = SecurityRoleNames.SysAdmin)]
        public ResultDto DeleteRole(long id)
        {
            return Result(() => {
                var command = new DeleteRoleCommand
                {
                    Id = id
                };
                CommandDispatcher.Send(command);
            });            
        }
    }
}
