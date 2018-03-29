using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Roham.Lib.Validation;
using Roham.Lib.Domain.Persistence;
using Roham.Lib.Logger;

namespace Roham.Lib.Domain.CQS.Query
{
    public abstract class AbstractQueryHandlerBase
    {
        protected readonly IPersistenceUnitOfWorkFactory _uowFactory;
        protected IEntityMapperFactory _entityMapperFactory;

        protected AbstractQueryHandlerBase(
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory entityMapperFactory)
        {
            _uowFactory = uowFactory;
            _entityMapperFactory = entityMapperFactory;
        }

        protected bool IsValid<TQuery>(TQuery query)
            where TQuery : class
        {
            try
            {
                ValidatorUtil.Validate(query);
                return true;
            }
            catch (ValidationException)
            {
                return false;
            }
        }

        protected string GetValidationErrors<TQuery>(TQuery query)
        {

            string result = "";
            List<ValidationResult> errors;
            if (!ValidatorUtil.TryValidate(this, out errors))
            {
                string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage));
            }
            return result;
        }


        protected void CheckContract<TQuery, TResult>(TQuery query)
            where TQuery : class
        {
            Objects.Requires<ArgumentNullException>(query != null);
            Objects.Requires(IsValid(query), () => new ValidationException(GetValidationErrors(query)));
            Contract.Ensures(Contract.Result<TResult>() != null);
        }
    }

    public abstract class AbstractQueryHandler<TQuery, TResult> : AbstractQueryHandlerBase, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<AbstractQueryHandler<TQuery, TResult>>();

        protected AbstractQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory) { }

        public TResult Handle(TQuery query)
        {
            Log.DebugMethodParams(() => query);
            CheckContract<TQuery, TResult>(query);

            return OnHandle(query);
        }

        protected abstract TResult OnHandle(TQuery query);
    }

    public abstract class AbstractPagedQueryHandler<TQuery, TResult> : AbstractQueryHandlerBase, IPagedQueryHandler<TQuery, TResult>
       where TQuery : class, IPagedQuery<TResult>
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<AbstractPagedQueryHandler<TQuery, TResult>>();

        protected AbstractPagedQueryHandler(
            IPersistenceUnitOfWorkFactory uowFactory,
            IEntityMapperFactory entityMapperFactory) : base(uowFactory, entityMapperFactory) { }

        public PagedResult<TResult> Handle(int skip, int take, TQuery query)
        {
            Log.DebugMethodParams(() => query);
            CheckContract<TQuery, PagedResult<TResult>>(query);

            return OnHandle(skip, take, query);
        }

        protected abstract PagedResult<TResult> OnHandle(int skip, int take, TQuery query);
    }
}
