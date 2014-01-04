using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using WebMatrix.WebData;

namespace SocialApp.Controllers
{
    public class HomeController : Controller
    {
        private SocialAppContext db;

        public HomeController(SocialAppContext db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Show", "User", new { id = WebSecurity.GetUserId(User.Identity.Name) });
            }
            return View();
        }
    }
}
