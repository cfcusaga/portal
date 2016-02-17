using System.Web.Mvc;

namespace  Cfcusaga.Web.Controllers {
    public class HomeController : Controller {
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult About() {
            //ViewBag.Message = "Your app description page.";
            ViewBag.Message = "CFC USA-GA Events and Membership Portal";//"CFC USA-GAhttp://couplesforchristusa.org/sea/ga/";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
