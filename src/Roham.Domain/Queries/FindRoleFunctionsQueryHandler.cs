using System.Linq;
using System.Collections.Generic;
using Roham.Contracts.Dtos;
using Roham.Contracts.Queries;
using Roham.Domain.Entities.Security;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Domain;
using Roham.Lib.Ioc;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindRoleFunctionsQueryHandler : AbstractQueryHandler<FindRoleFunctionsQuery, RoleFunctionsDto>
    {
        public FindRoleFunctionsQueryHandler(
           IPersistenceUnitOfWorkFactory uowFactory,
           IEntityMapperFactory mapperFactory) : base(uowFactory, mapperFactory) { }

        protected override RoleFunctionsDto OnHandle(FindRoleFunctionsQuery query)
        {
            var roleId = query.RoleId;
            var result = new RoleFunctionsDto
            {   
                Functions = new List<AppFunctionDto>()
            };

            var mapper = _entityMapperFactory.Create<AppFunctionDto, AppFunction>();
            var roleMapper = _entityMapperFactory.Create<RoleDto, Role>();
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var role = uow.Context.FindById<Role>(roleId);                
                var roleDto = roleMapper.Map(role);
                result.Fill(roleDto);

                var roleAppFuncs = role.AppFunctions.ToDictionary(a => a.Id);
                var allAppFuncs = uow.Context.All<AppFunction>().ToDictionary(a => a.Id);

                foreach(var appFuncEntry in allAppFuncs)
                {
                    var appFuncDto = mapper.Map(appFuncEntry.Value);                    
                    appFuncDto.IsAllowed = roleAppFuncs.ContainsKey(appFuncEntry.Key);
                    result.Functions.Add(appFuncDto);
                }
                uow.Complete();
            }

            return result;
        }
    }
}
