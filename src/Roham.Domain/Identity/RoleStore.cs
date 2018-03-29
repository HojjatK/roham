using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Domain.Entities.Security;
using Roham.Domain.Mappers;
using Roham.Contracts.Commands.Role;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Identity
{   
    public class RoleStore : IRoleStore<ApplicationRole, long>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandDispatcher _commandDispatcher;

        public RoleStore(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher)
        {
            _queryExecutor = queryExecutor;
            _commandDispatcher = commandDispatcher;
        }

        public Task<ApplicationRole> FindByIdAsync(long roleId)
        {
            RoleDto roleDto = _queryExecutor.Execute(context =>
            {
                var role = context
                    .FindById<Role>(roleId);
                return new RoleMapper().Map(role);
            });

            return Task<ApplicationRole>
                .FromResult(new ApplicationRole(roleDto));
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            RoleDto roleDto = _queryExecutor.Execute(context =>
            {
                var role = context
                    .Query<Role>()
                    .SingleOrDefault(r => r.Name == roleName);
                return new RoleMapper().Map(role);
            });

            return Task<ApplicationRole>
                .FromResult(new ApplicationRole(roleDto));
        }

        public Task CreateAsync(ApplicationRole role)
        {
            _commandDispatcher.Send(new AddRoleCommand
            {
                Name = role?.Details.Name,
                Description = role?.Details.Description,
                RoleType = role.Details == null ? null : role.Details.RoleType,
            });
            return Task.FromResult(0);
        }

        public Task UpdateAsync(ApplicationRole role)
        {
            _commandDispatcher.Send(new UpdateRoleCommand
            {
                Id = role.Id,
                Description = role?.Details.Description
            });
            return Task.FromResult(0);
        }

        public Task DeleteAsync(ApplicationRole role)
        {
            _commandDispatcher.Send(new DeleteRoleCommand
            {
                Id = role.Id,
            });
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }
    }
}
