using System.Web.Mvc;
using DataAccess;

namespace SocialApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly SocialAppContext db;

        public MenuController(SocialAppContext db)
        {
            this.db = db;
        }

        public PartialViewResult TopBar()
        {
            return PartialView();
        }

    }
}
