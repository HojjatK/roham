namespace Roham.Lib.Domain.CQS.Query
{
    public interface IQueryHandler<in TQuery, out TResult>
        where TQuery : class, IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    public interface IPagedQueryHandler<in TQuery, TResult>
        where TQuery : class, IPagedQuery<TResult>
    {
        PagedResult<TResult> Handle(int skip, int take, TQuery query);
    }
}
