using System.Linq;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using Roham.Contracts.Queries;
using Roham.Domain.Services;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.Cache;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindByNameQueryHandler<TDto, TEntity> : AbstractQueryHandler<FindByNameQuery<TDto, TEntity>, TDto>
        where TEntity : Identifiable, INamed
        where TDto : CachableDto
    {
        private readonly ICacheService _cacheService;

        public FindByNameQueryHandler(
            ICacheService cacheService,
            IPersistenceUnitOfWorkFactory uowFactory, 
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory)
        {
            _cacheService = cacheService;
        }

        protected override TDto OnHandle(FindByNameQuery<TDto, TEntity> query) 
        {
            var cacheIndex = CacheKey.NewIndex<TDto, string>(nameof(query.Name), query.Name);            
            return _cacheService.Get(cacheIndex, () =>
            {
                TDto result;
                using (var uow = _uowFactory.CreateReadOnly())
                {
                    var name = query.Name;
                    var entity = uow.Context.Query<TEntity>().SingleOrDefault(s => s.Name == name);
                    result = _entityMapperFactory.Create<TDto, TEntity>().Map(entity);

                    uow.Complete();
                }
                return result;
            });
        }
    }
}
