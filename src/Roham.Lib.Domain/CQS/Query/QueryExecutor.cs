using Roham.Lib.Domain.Persistence;
using Roham.Lib.Ioc;
using System;

namespace Roham.Lib.Domain.CQS.Query
{
    public interface IQueryExecutor
    {
        T Execute<T>(IQuery<T> query);
        PagedResult<T> Execute<T>(int skip, int take, IPagedQuery<T> query);

        TResult Execute<TResult>(Func<IPersistenceContext, TResult> queryAction);
    }

    [AutoRegister(LifetimeScope = LifetimeScopeType.InstancePerLifetimeScope)]
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ILifetimeScope _lifetimeScope;

        public QueryExecutor(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic queryHandler = _lifetimeScope.Resolve(handlerType);            

            return queryHandler.Handle((dynamic)query);
        }

        public PagedResult<TResult> Execute<TResult>(int pageNumber, int itemsPerPage, IPagedQuery<TResult> query)
        {
            var handlerType = typeof(IPagedQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic queryHandler = _lifetimeScope.Resolve(handlerType);

            int skip = (pageNumber - 1) * itemsPerPage;
            int take = itemsPerPage;
            return queryHandler.Handle(skip, take, (dynamic)query) as PagedResult<TResult>;
        }

        public TResult Execute<TResult>(Func<IPersistenceContext, TResult> queryAction)
        {
            TResult result = default(TResult);
            var uowFactory = _lifetimeScope.Resolve<IPersistenceUnitOfWorkFactory>();
            using(var uow = uowFactory.Create())
            {
                result = queryAction(uow.Context);
                uow.Complete();
            }
            return result;
        }
    }
}