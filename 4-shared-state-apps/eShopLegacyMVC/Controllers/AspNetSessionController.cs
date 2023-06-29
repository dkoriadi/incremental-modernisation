using eShopLegacy.Models;
using log4net;
using System.Web.Mvc;

namespace eShopLegacyMVC.Controllers
{
    public class AspNetSessionController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: AspNetSession
        public ActionResult Index()
        {
            // START: Testing session variables between ASP.NET and Core applications
            if (System.Web.HttpContext.Current?.Session?["test-value"] != null)
            {
                var value = System.Web.HttpContext.Current?.Session?["test-value"] as int?;
                _log.Info("[Legacy] MVC Legacy session value: " + value);
            }
            System.Web.HttpContext.Current.Session["test-value"] = 1;
            // END: Testing session variables between ASP.NET and Core applications
            var model = HttpContext.Session["DemoItem"];
            return View(model);
        }

        // POST: AspNetSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SessionDemoModel demoModel)
        {
            HttpContext.Session["DemoItem"] = demoModel;
            return View(demoModel);
        }
    }
}
