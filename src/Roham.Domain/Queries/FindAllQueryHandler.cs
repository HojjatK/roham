using System.Linq;
using System.Collections.Generic;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain.Persistence;
using Roham.Contracts.Queries;
using Roham.Lib.Ioc;
using Roham.Contracts.Dtos;

namespace Roham.Domain.Queries
{
    [AutoRegister]
    public class FindAllQueryHandler<TDto, TEntity> : AbstractQueryHandler<FindAllQuery<TDto, TEntity>, List<TDto>>
        where TEntity : AggregateRoot
        where TDto : CachableDto
    {
        public FindAllQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory, 
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory)
        {
        }

        protected override List<TDto> OnHandle(FindAllQuery<TDto, TEntity> query)
        {
            var result = new List<TDto>();
            using (var uow = _uowFactory.CreateReadOnly())
            {
                var entities = uow.Context.All<TEntity>().ToList();
                entities.ForEach(entity => result.Add(_entityMapperFactory.Create<TDto, TEntity>().Map(entity)));

                uow.Complete();
            }
            return result;
        }
    }
}
