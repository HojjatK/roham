using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using System.Collections.Generic;
using System.Linq;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindUserEntryPermissionsQueryHandler : AbstractQueryHandler<FindUserEntryPermissionsQuery, List<UserPostPermissionDto>>
    {
        public FindUserEntryPermissionsQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override List<UserPostPermissionDto> OnHandle(FindUserEntryPermissionsQuery query)
        {
            var userId = query.UserId;
            var result = new List<UserPostPermissionDto>();

            var mapper = _entityMapperFactory.Create<UserPostPermissionDto, PostPermission>();
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var user = uow.Context.FindById<User>(userId);
                var permissions = user.Permissions.ToList();

                foreach (var perm in permissions)
                {
                    result.Add(mapper.Map(perm));
                };

                uow.Complete();
            }

            return result;
        }
    }
}
