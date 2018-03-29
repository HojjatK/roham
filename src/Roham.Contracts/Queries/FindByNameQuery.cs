using Roham.Contracts.Dtos;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;

namespace Roham.Contracts.Queries
{
    public class FindByNameQuery<TDto, TEntity> : IQuery<TDto>
        where TEntity : Identifiable, INamed
        where TDto : CachableDto
    {
        public FindByNameQuery(string name)
        {
            Name = name;
        }

        public string QueryString => $"name:{Name}";

        public string Name { get; }

        public override string ToString()
        {
            return $"Find {typeof(TEntity).Name} [{QueryString}]";
        }
    }
}
