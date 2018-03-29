using System.Collections.Generic;
using Roham.Lib.Domain.CQS.Query;
using Roham.Lib.Domain;
using Roham.Contracts.Dtos;

namespace Roham.Contracts.Queries
{
    public class FindAllQuery<TDto, TEntity> : IQuery<List<TDto>>
        where TEntity : AggregateRoot
        where TDto : CachableDto
    {
        public string QueryString => "";

        public override string ToString()
        {
            return $@"FindAll {typeof(TEntity).Name}";
        }
    }
}
