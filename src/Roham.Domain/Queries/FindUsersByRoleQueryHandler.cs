using System;
using System.Linq;
using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Ioc;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindUsersByRoleQueryHandler : AbstractQueryHandler<FindUsersByRoleQuery, List<UserDto>>
    {
        public FindUsersByRoleQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override List<UserDto> OnHandle(FindUsersByRoleQuery query)
        {
            var roleTypeCode = (RoleTypeCodes)Enum.Parse(typeof(RoleTypeCodes), query.RoleType, true);
            var result = new List<UserDto>();

            var mapper = _entityMapperFactory.Create<UserDto, User>();
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var users = uow.Context.Query<User>()
                                      .Where(u => u.Roles.Any(r => r.RoleType == roleTypeCode))
                                      .ToList();

                foreach (var user in users)
                {
                    result.Add(mapper.Map(user));
                };

                uow.Complete();
            }

            return result;

        }
    }
}