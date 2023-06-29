using eShopLegacy.Models;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace eShopLegacyMVCCore.Controllers
{
    public class AspNetCoreSessionController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: AspNetCoreSession
        public ActionResult Index()
        {
            var model = ((System.Web.HttpContext)HttpContext).Session["DemoItem"];
            return View(model);
        }

        // POST: AspNetCoreSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SessionDemoModel demoModel)
        {
            ((System.Web.HttpContext)HttpContext).Session["DemoItem"] = demoModel;
            return View(demoModel);
        }
    }
}
