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
    public class FindByUserNameQueryHandler<TDto, TEntity> : AbstractQueryHandler<FindByUserNameQuery<TDto, TEntity>, TDto>
        where TEntity : AggregateRoot, IUserNamed
        where TDto : CachableDto
    {
        private readonly ICacheService _cacheService;

        public FindByUserNameQueryHandler(
            ICacheService cacheService,
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory)
        {
            _cacheService = cacheService;
        }

        protected override TDto OnHandle(FindByUserNameQuery<TDto, TEntity> query)
        {
            var cacheIndex = CacheKey.NewIndex<TDto, string>(nameof(query.UserName), query.UserName);            
            return _cacheService.Get(cacheIndex, () =>
            {
                TDto result;
                using (var uow = _uowFactory.CreateReadOnly())
                {
                    var userName = query.UserName;
                    var entity = uow.Context.Query<TEntity>().SingleOrDefault(s => s.UserName == userName);
                    result = _entityMapperFactory.Create<TDto, TEntity>().Map(entity);

                    uow.Complete();
                }
                return result;
            });
        }
    }
}
