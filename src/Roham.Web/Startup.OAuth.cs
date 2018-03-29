using System;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Roham.Domain.Configs;
using Roham.Domain.Identity;

namespace Roham.Web
{
    public partial class Startup
    {
        private void ConfigureOAuth(IAppBuilder app, IRohamConfigs rohamConfigs)
        {
            app.CreatePerOwinContext<ApplicationUserManager>(CreateUserManager);
            app.CreatePerOwinContext<ApplicationRoleManager>(CreateRoleManager);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            var cookieAuthOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login"),
            };

            app.UseCookieAuthentication(cookieAuthOptions);
            if (rohamConfigs.IsInstalled)
            {
                cookieAuthOptions.Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, long>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie),
                        getUserIdCallback: c => c.GetUserId<long>())
                };
            }
            else
            {
                cookieAuthOptions.Provider = new CookieAuthenticationProvider();
            }

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "1",
            //    clientSecret: "1");

            app.UseTwitterAuthentication(
               consumerKey: "1",
               consumerSecret: "1");

            app.UseFacebookAuthentication(
               appId: "1",
               appSecret: "1");

            app.UseGoogleAuthentication(
                clientId: "104443523592-pdhb03d6bfg3m87n2c69ue3g6narugpf.apps.googleusercontent.com",
                clientSecret: "pChV00MKKcUtsnjvLg1Rwkvl");
        }

        private static ApplicationRoleManager CreateRoleManager(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            var roleStore = RohamDependencyResolver.Current.Resolve<IRoleStore<ApplicationRole, long>>();
            return new ApplicationRoleManager(roleStore);
        }

        private static ApplicationUserManager CreateUserManager(
          IdentityFactoryOptions<ApplicationUserManager> options,
          IOwinContext context)
        {
            var userStore = RohamDependencyResolver.Current.Resolve<IUserStore<ApplicationUser, long>>();
            var manager = new ApplicationUserManager(userStore);
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            manager.PasswordHasher = new RohamPasswordHasher();

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser, long>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser, long>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = RohamDependencyResolver.Current.Resolve<EmailService>();
            manager.SmsService = RohamDependencyResolver.Current.Resolve <SmsService>();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
