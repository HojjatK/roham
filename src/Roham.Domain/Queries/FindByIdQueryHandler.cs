using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Queries;
using Roham.Domain.Services;
using Roham.Lib.Ioc;
using Roham.Contracts.Dtos;
using Roham.Lib.Domain.Cache;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindByIdQueryHandler<TDto, TEntity> : AbstractQueryHandler<FindByIdQuery<TDto, TEntity>, TDto>
        where TEntity : AggregateRoot
        where TDto : CachableDto
    {
        private readonly ICacheService _cacheService;

        public FindByIdQueryHandler(
            ICacheService cacheService, 
            IPersistenceUnitOfWorkFactory uowFactory, 
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory)
        {
            _cacheService = cacheService;
        }

        protected override TDto OnHandle(FindByIdQuery<TDto, TEntity> query)
        {
            var cacheKey = CacheKey.New<TDto, long>(nameof(query.Id), query.Id);
            return _cacheService.Get(cacheKey, () =>
            {
                TDto result;
                using (var uow = _uowFactory.CreateReadOnly())
                {
                    var entity = uow.Context.FindById<TEntity>(query.Id);
                    result = _entityMapperFactory.Create<TDto, TEntity>().Map(entity);

                    uow.Complete();
                }
                return result;
            });
        }
    }
}
