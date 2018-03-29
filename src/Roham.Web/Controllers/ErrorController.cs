using System.Web.Mvc;
using Roham.Web.Mvc.Filters;
using Roham.Resources;
using Roham.Domain.Services;
using Roham.Lib.Caches;
using Roham.Lib.Domain.Cache;

namespace Roham.Web.Controllers
{
    [RoutePrefix("error")]
    [LogActions]
    public class ErrorController : Controller
    {
        private readonly ICacheService _cacheService;

        public ErrorController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult CustomError()
        {   
            ViewBag.ErrorMessage = GetErrorMessage();
            return View("CustomError");
        }

        private string GetErrorMessage()
        {   
            var errorKey = Request.QueryString.Get("code");
            string error = null;
            string errorMessage = "", expMessage = null;
            if (!_cacheService.MemoryCache.TryGet(errorKey, out error))
            {
                errorMessage = ErrorMessages.SystemErrorOccured;
            }
            else
            {
                errorMessage = error ?? ErrorMessages.SystemErrorOccured;
                if (_cacheService.MemoryCache.TryGet($"{errorKey}-exp", out expMessage))
                {
                    ViewBag.ExpMessage = expMessage;
                }
            }
            
            return errorMessage;
        }
    }
}