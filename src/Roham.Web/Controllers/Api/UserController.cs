using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Contracts.Commands.User;

namespace Roham.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class UserController : ApiControllerBase
    {
        public UserController(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher) : base(queryExecutor, commandDispatcher) {}

        [HttpGet]
        [Route("")]
        public List<UserDto> GetUsers()
        {
            return QueryExecutor.Execute(new FindAllQuery<UserDto, User>());
        }

        [HttpGet]
        [Route("by-role/{roleType}")]
        public List<KeyValuePair<string, string>> GetUsersByRole(string roleType)
        {
            var result = new List<KeyValuePair<string, string>>();
            var roleTypeCode = RoleTypeCodes.User;
            if (Enum.TryParse<RoleTypeCodes>(roleType, out roleTypeCode))
            {
                var users = QueryExecutor.Execute(new FindUsersByRoleQuery(roleType));
                users.ForEach(u => result.Add(new KeyValuePair<string, string>(u.Id.ToString(), u.UserName)));
            }
            return result;
        }

        [HttpGet]
        [Route("{id:long}")]
        public UserDto GetUser(long id)
        {
            return QueryExecutor.Execute(new FindByIdQuery<UserDto, User>(id));
        }

        [HttpGet]
        [Route("{id:long}/permissions")]
        public List<UserPostPermissionDto> GetUserEntryPermissions(long id)
        {
            return QueryExecutor.Execute(new FindUserEntryPermissionsQuery { UserId = id });
        }

        [HttpPost]        
        [Route("")]
        public ResultDto CreateUser(UserDto newUser)
        {
            return Result(() =>
            {
                var roleIds = newUser.RoleIdNames != null ? newUser.RoleIdNames.Select(i => i.Id).ToList() : new List<long>();
                var siteIds = newUser.SiteIdNames != null ? newUser.SiteIdNames.Select(i => i.Id).ToList() : new List<long>();

                var command = new AddUserCommand
                {
                    Email = newUser.Email,
                    UserName = newUser.UserName,
                    IsSystemUser = newUser.IsSystemUser,
                    Title = newUser.Title,
                    GivenName = newUser.GivenName,
                    MiddleName = "",
                    Surname = newUser.Surname,
                    PhoneNumber = newUser.PhoneNumber,
                    RoleIds = roleIds,
                    SiteIds = siteIds
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpPut]        
        [Route("{id:long}")]
        public ResultDto UpdateUser(long id, UserDto user)
        {
            return Result(() =>
            {
                var roleIds = user.RoleIdNames != null ? user.RoleIdNames.Select(i => i.Id).ToList() : new List<long>();
                var siteIds = user.SiteIdNames != null ? user.SiteIdNames.Select(i => i.Id).ToList() : new List<long>();

                var command = new UpdateUserCommand
                {
                    Id = id,
                    Title = user.Title,
                    GivenName = user.GivenName,
                    MiddleName = "",
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber ?? "",
                    RoleIds = roleIds,
                    SiteIds = siteIds
                };
                CommandDispatcher.Send(command);
            });
        }

        [HttpDelete]        
        [Route("{id:long}")]
        public ResultDto DeleteUser(long id)
        {
            return Result(() => {
                var command = new DeleteUserCommand
                {
                    Id = id
                };
                CommandDispatcher.Send(command);
            });
        }

    }
}
