using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eShopLegacyMVCCore.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Scheme must be the same as cookie set in Program.cs
            HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect("/");
        }
    }
}
