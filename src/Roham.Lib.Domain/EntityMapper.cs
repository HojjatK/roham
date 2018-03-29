using Roham.Lib.Ioc;

namespace Roham.Lib.Domain
{
    public interface IEntityMapper<DTO, ENTITY>
    {
        DTO Map(ENTITY entity);
    }

    public interface IEntityMapperFactory
    {
        IEntityMapper<DTO, ENTITY> Create<DTO, ENTITY>();
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.InstancePerLifetimeScope)]
    public class EntityMapperFactory : IEntityMapperFactory
    {
        private readonly IResolver _resolver;

        public EntityMapperFactory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public IEntityMapper<DTO, ENTITY> Create<DTO, ENTITY>()
        {
            return _resolver.Resolve<IEntityMapper<DTO, ENTITY>>();
        }
    }
}
