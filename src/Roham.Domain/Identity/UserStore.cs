using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Domain.Entities.Security;
using Roham.Domain.Mappers;
using Roham.Contracts.Dtos;
using Roham.Contracts.Commands.User;
using Roham.Lib.Cryptography;
using Roham.Contracts.Queries;

namespace Roham.Domain.Identity
{   
    public class UserStore :
        IUserStore<ApplicationUser, long>,
        IUserRoleStore<ApplicationUser, long>,
        IUserPasswordStore<ApplicationUser, long>,
        IUserClaimStore<ApplicationUser, long>,
        IUserSecurityStampStore<ApplicationUser, long>,
        IUserEmailStore<ApplicationUser, long>,
        IUserLoginStore<ApplicationUser, long>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandDispatcher _commandDispatcher;

        public UserStore(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher)
        {
            _queryExecutor = queryExecutor;
            _commandDispatcher = commandDispatcher;
        }

        public Task<ApplicationUser> FindByIdAsync(long userId)
        {
            var userDto = _queryExecutor.Execute(new FindByIdQuery<UserDto, User>(userId));
            return Task.FromResult(userDto != null ? new ApplicationUser(userDto) : null);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var userDto = _queryExecutor.Execute(new FindByUserNameQuery<UserDto, User>(userName));
            return Task.FromResult(userDto != null ? new ApplicationUser(userDto) : null);
        }

        public Task CreateAsync(ApplicationUser user)
        {
            _commandDispatcher.Send(new AddUserCommand
            {
                UserName = user.UserName,
                IsSystemUser = user.Details.IsSystemUser,
                Title = user.Details.Title,
                GivenName = user.Details.GivenName,
                Surname = user.Details.Surname,
                Email = user.Details.Email,
                PhoneNumber = user.Details.PhoneNumber,
                RoleIds = user.Details.RoleIdNames.Select(item => item.Id).ToList(),
            });
            return Task.FromResult(0);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                Title = user?.Details.Title,
                GivenName = user?.Details.GivenName,
                Surname = user?.Details.Surname,
            };
            if (command.IsIndividual)
            {
                _commandDispatcher.Send(command);
            }
            return Task.FromResult(0);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            _commandDispatcher.Send(new DeleteUserCommand
            {
                Id = user.Id,
            });
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            IList<string> roleNames = _queryExecutor.Execute(context =>
            {
                long userId = user.Id;
                var types = context
                    .Query<User>()
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Roles)
                    .Select(r => r.RoleType)
                    .ToList()
                    .Distinct();

                return types.Select(t => SecurityRoleNames.GetSecurityRoleName(t)).ToList();
            });

            return Task.FromResult(roleNames);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            var roleNames = GetRolesAsync(user);
            bool isInRole = roleNames.Result.Contains(roleName);
            return Task.FromResult(isInRole);
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            _commandDispatcher.Send(new AddUserRoleCommand
            {
                UserId = user.Id,
                RoleName = roleName
            });
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            _commandDispatcher.Send(new DeleteUserRoleCommand
            {
                UserId = user.Id,
                RoleName = roleName
            });
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Details.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            string passwordHash = user.Details.PasswordHash;
            return Task.FromResult(passwordHash != null && !string.IsNullOrWhiteSpace(passwordHash));
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {   
            _commandDispatcher.Send(new ChangePasswordCommand
            {
                UserId = user.Id,                
                PasswordHashAlgorithm = HashAlgorithm.PBKDF2.ToString(),
                NewPasswordHash = passwordHash,
            });
            return Task.FromResult(0);
        }

        public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(ApplicationUser user)
        {
            IList<System.Security.Claims.Claim> claims = _queryExecutor.Execute(context =>
            {
                var userId = user.Id;
                return context.Query<User>()
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.UserClaims)
                    .ToList()
                    .Select(uc => new System.Security.Claims.Claim(uc.ClaimType, uc.ClaimValue))
                    .ToList();
            });

            return Task.FromResult(claims);
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            _commandDispatcher.Send(new AddUserClaimCommand
            {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                ClaimValueType = claim.ValueType,
            });

            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            _commandDispatcher.Send(new DeleteUserClaimCommand
            {
                UserId = user.Id,
                ClaimType = claim.Type,
            });

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Details.SecurityStamp);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            if (user.Id != 0)
            {
                // User.id is zero when user has been not created yet
                _commandDispatcher.Send(new SetSecurityStampCommand
                {
                    UserId = user.Id,
                    SecurityStamp = stamp,
                });
            }


            return Task.FromResult(0);
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            _commandDispatcher.Send(new SetUserEmailCommand
            {
                UserId = user.Id,
                Email = email,
            });

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Details.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Details.EmailConfirm);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            _commandDispatcher.Send(new SetUserEmailConfirmCommand
            {
                UserId = user.Id,
                Confirmed = confirmed,
            });

            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            UserDto userDto = _queryExecutor.Execute(context =>
            {
                var user = context
                    .Query<User>()
                    .SingleOrDefault(u => u.Email == email);
                return new UserMapper().Map(user);
            });

            return Task.FromResult(userDto != null ? new ApplicationUser(userDto) : null);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            _commandDispatcher.Send(new AddUserLoginCommand
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
            });

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            _commandDispatcher.Send(new RemoveUserLoginCommand
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
            });

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            IList<UserLoginInfo> loginInfo = _queryExecutor.Execute(context =>
            {
                var userId = user.Id;
                var userEntity = context
                    .FindById<User>(userId);

                return userEntity.UserLogins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            });
            return Task.FromResult(loginInfo);
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            ApplicationUser appUser = _queryExecutor.Execute(context =>
            {
                var user = context
                    .Query<User>()
                    .SingleOrDefault(u => u.UserLogins.Any(ul => ul.LoginProvider == login.LoginProvider && ul.ProviderKey == login.ProviderKey));
                return user != null ? new ApplicationUser(new UserMapper().Map(user)) : null;
            });
            return Task.FromResult(appUser);
        }

        public void Dispose()
        {
        }
    }
}
