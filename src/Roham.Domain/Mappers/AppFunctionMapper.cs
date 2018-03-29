using Roham.Lib.Domain;
using Roham.Domain.Entities.Security;
using Roham.Contracts.Dtos;
using Roham.Lib.Ioc;

namespace Roham.Domain.Mappers
{
    [AutoRegister(LifetimeScope = LifetimeScopeType.SingleInstance)]
    public class AppFunctionMapper : IEntityMapper<AppFunctionDto, AppFunction>
    {
        public AppFunctionDto Map(AppFunction appFunction)
        {
            return new AppFunctionDto
            {
                Uid = appFunction.Uid.ToString(),
                Id = appFunction.Id,
                Name = appFunction.Key.ToString(),
                DisplayName = appFunction.Title,
                Description = appFunction.Description,                
                Parent = appFunction.Parent != null ? Map(appFunction.Parent) : null,
            };
        }
    }
}
