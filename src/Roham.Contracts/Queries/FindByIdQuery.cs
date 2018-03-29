using System.ComponentModel.DataAnnotations;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindByIdQuery<TDto, TEntity> : IQuery<TDto>
        where TEntity : AggregateRoot
        where TDto : CachableDto
    {
        public FindByIdQuery(long id)
        {
            Id = id;
        }

        public string QueryString => $"id:{Id}";

        [Range(1, long.MaxValue)]
        public long Id { get; private set; }

        public override string ToString()
        {
            return $"Find {typeof(TEntity).Name} [{QueryString}]";
        }
    }
}
