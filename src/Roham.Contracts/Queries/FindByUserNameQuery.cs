using Roham.Contracts.Dtos;
using Roham.Lib.Domain;
using Roham.Lib.Domain.CQS.Query;

namespace Roham.Contracts.Queries
{
    public class FindByUserNameQuery<TDto, TEntity> : IQuery<TDto>
        where TEntity : AggregateRoot, IUserNamed
        where TDto : CachableDto
    {
        public FindByUserNameQuery(string userName)
        {
            UserName = userName;
        }

        public string QueryString => $"username:{UserName}";

        public string UserName { get; }

        public override string ToString()
        {
            return $"Find {typeof(TEntity).Name} [{QueryString}]";
        }
    }
}
