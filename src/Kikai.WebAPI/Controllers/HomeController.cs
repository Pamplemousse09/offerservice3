using System.Web.Mvc;

namespace Kikai.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Offer Service";
            return View();
        }
    }
}
