using System.Web.Mvc;

namespace SocialApp.Controllers
{
    public class MenuController : Controller
    {
        public PartialViewResult TopBar()
        {
            return PartialView();
        }

    }
}
