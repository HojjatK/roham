using System.Linq;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class UserMapper : IEntityMapper<UserDto, User>
    {
        public UserDto Map(User user)
        {
            if (user == null)
            {
                return null;
            }
            return new UserDto
            {
                Uid = user.Uid.ToString(),
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirm = user.EmailConfirm,
                Title = user.Person != null ? user.Person.Title : "",
                GivenName = user.Person != null ? user.Person.GivenName : "",
                Surname = user.Person != null ? user.Person.Surname : "",
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                SecurityStamp = user.SecurityStamp,
                AccessFailedCount = user.AccessFailedCount,
                IsSystemUser = user.IsSystemUser,
                Status = user.Status.ToString(),
                StatusReason = user.StatusReason,
                PasswordHashAlgorithm = user.PasswordHashAlgorithm,
                PasswordHash = user.PasswordHash,
                SiteIdNames = user.GetUserSites().Select(s => new { s.Id, s.Title }).ToList().Select(item => new IdNamePair { Id = item.Id, Name = item.Title }).ToList(),
                RoleIdNames = user.Roles.Select(r => new { r.Id, r.Name }).ToList().Select(item => new IdNamePair { Id = item.Id, Name = item.Name }).ToList(),
            };
        }
    }
}
