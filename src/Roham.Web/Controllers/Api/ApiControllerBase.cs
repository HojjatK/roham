using System;
using System.Linq;
using System.Web.Http;
using Roham.Lib.Domain.CQS.Command;
using Roham.Lib.Domain.CQS.Query;
using Roham.Web.Mvc.Filters;
using Roham.Contracts.Dtos;
using System.ComponentModel.DataAnnotations;
using Roham.Domain.Entities.Security;
using Microsoft.AspNet.Identity;
using Roham.Contracts.Queries;

namespace Roham.Web.Controllers.Api
{
    [ApiLogActions]
    public abstract class ApiControllerBase : ApiController
    {
        protected ApiControllerBase(
            IQueryExecutor queryExecutor,
            ICommandDispatcher commandDispatcher)
        {
            QueryExecutor = queryExecutor;
            CommandDispatcher = commandDispatcher;
        }

        protected IQueryExecutor QueryExecutor { get; }
        protected ICommandDispatcher CommandDispatcher { get; }

        protected ResultDto Result(Action action)
        {
            var result = new ResultDto { Succeed = true };
            try
            {
                if (ModelState.IsValid)
                {
                    action();
                }
                else
                {
                    result.Succeed = false;
                    result.ErrorMessages.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }
            }
            catch(ValidationException exp)
            {
                result.Succeed = false;
                result.ErrorMessages.Add(exp.Message);
            }
            return result;
        }

        protected UserDto GetCurrentSessionUser()
        {
            var currentUserName = User.Identity.GetUserName();
            if (!string.IsNullOrWhiteSpace(currentUserName))
            {
                return QueryExecutor.Execute(new FindByUserNameQuery<UserDto, User>(currentUserName));
            }

            return null;
        }
    }
}
