using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Interop;
using Owin;
using eShopLegacyMVC.Models;
using Microsoft.Owin.Security.DataProtection;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace eShopLegacyMVC
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Shared cookie authentication
            // These must match the data protection settings in MvcCoreApp Program.cs for cookie sharing to work
            var keyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DPKeys");
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = ".AspNet.ApplicationCookie",
                CookieSameSite = SameSiteMode.Lax,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromDays(1),
                // Settings to configure shared cookie with MvcCoreApp
                TicketDataFormat = new AspNetTicketDataFormat(new DataProtectorShim(
                    DataProtectionProvider.Create(new DirectoryInfo(keyPath),
                    (builder) =>
                    {
                        builder.SetApplicationName("eShop");
                    })
                    .CreateProtector(
                        "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                        // Must match the Scheme name on the MvcCoreApp, i.e. IdentityConstants.ApplicationScheme
                        "IdentityConstants.ApplicationScheme",
                        "v2"
                        )
                    )
                ),
                Provider = new CookieAuthenticationProvider
                    {
                        // Enables the application to validate the security stamp when the user logs in.
                        // This is a security feature which is used when you change a password or add an external login to your account.  
                        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                    },
            });
        }
    }
}