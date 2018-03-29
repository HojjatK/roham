using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Roham.Web.ViewModels;

namespace Roham.Web.Controllers
{
    public partial class AccountController
    {
        [Route("Manage")]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var userId = long.Parse(User.Identity.GetUserId());
            var user = UserManager.FindById(userId);

            ViewBag.HasLocalPassword = user != null && user.Details != null && user.Details.PasswordHash != null;
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        // TODO: send change password command
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Manage")]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            var userId = long.Parse(User.Identity.GetUserId());
            var user = UserManager.FindById(userId);
            bool hasPassword = user != null && user.Details != null && user.Details.PasswordHash != null;

            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(userId, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}
