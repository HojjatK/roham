using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Dtos;
using Roham.Lib.Ioc;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class UserEntryPermissionMapper : IEntityMapper<UserPostPermissionDto, PostPermission>
    {
        public UserPostPermissionDto Map(PostPermission entryPerm)
        {
            return new UserPostPermissionDto
            {
                UserId = entryPerm.User.Id,
                PostId = entryPerm.Post.Id,
                Title = entryPerm.Post.Title,
                Description = entryPerm.Post.Name,
                View = entryPerm.Read,
                Create = entryPerm.Create,
                Update = entryPerm.Update,
                Delete = entryPerm.Delete,
            };
        }
    }
}
