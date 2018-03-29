using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Roham.Web.ViewModels;
using Roham.Contracts.Dtos;
using Roham.Domain.Identity;
using Roham.Resources;

namespace Roham.Web.Controllers
{
    public partial class AccountController
    {
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("ExternalLogin")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        [Route("ExternalLoginCallback")]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                // If user already logged in, then prompt the user to associate current account with external provider
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("LinkLogin")]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        [Route("LinkLoginCallback")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var userId = long.Parse(User.Identity.GetUserId());
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(userId, loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("ExternalLoginConfirmation")]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }

            ApplicationUser user;
            if (User.Identity.IsAuthenticated)
            {
                // find existing user
                user = await UserManager.FindByNameAsync(User.Identity.Name);
                var logins = await UserManager.GetLoginsAsync(user.Id);
                if (logins != null && logins.Any(l => l.LoginProvider == info.Login.LoginProvider))
                {
                    // user is alreay associated
                    return RedirectToAction("Manage");
                }

                if (user.UserName != model.Email)
                {
                    ModelState.AddModelError(nameof(ErrorMessages.InvalidUser), ErrorMessages.InvalidUser);
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }
            }
            else
            {
                // create new user          
                var role = RoleManager.FindByName("Modertor"); 
                user = new ApplicationUser(new UserDto { UserName = model.Email, Email = model.Email, RoleIdNames = new List<IdNamePair> { new IdNamePair { Id = role.Id, Name = role.Name } } });
                var createResult = await UserManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    AddErrors(createResult);
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }
                // read user back from database, to refresh the id
                user = UserManager.FindByName(user.UserName);                
            }

            var result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (result.Succeeded)
            {
                await SignInAsync(user, isPersistent: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToLocal(returnUrl);
            }
            AddErrors(result);           

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        [Route("ExternalLoginFailure")]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        [Route("RemoveAccountList")]
        public ActionResult RemoveAccountList()
        {
            var userId = long.Parse(User.Identity.GetUserId());
            var linkedAccounts = UserManager.GetLogins(userId);
            var user = UserManager.FindById(userId);
            ViewBag.ShowRemoveButton = (user != null && user.Details != null && user.Details.PasswordHash != null) || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}
