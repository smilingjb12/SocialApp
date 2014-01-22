using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;

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
