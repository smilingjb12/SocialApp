using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using DataAccess;
using SocialApp.Models;
using WebMatrix.WebData;

namespace SocialApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly SocialAppContext db;

        public UserController(SocialAppContext db)
        {
            this.db = db;
        }

        public ActionResult Show(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Update(UserUpdateModel model)
        {
            User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
            if (model.Picture != null)
            {
                string extension = System.IO.Path.GetExtension(model.Picture.FileName);
                string relativePicturePath = string.Format("/{0}/{1}{2}", Strings.ProfilePicturesFolder, Guid.NewGuid(), extension);
                string serverPicturePath = Server.MapPath(string.Format("~/{0}", relativePicturePath));
                model.Picture.SaveAs(serverPicturePath);
                currentUser.PictureFilePath = relativePicturePath;
            }
            currentUser.About = model.About;
            currentUser.City = model.City;
            currentUser.Country = model.Country;
            currentUser.FullName = model.FullName;
            db.SaveChanges();

            return RedirectToAction("Settings");
        }

        public ViewResult Settings()
        {
            User currentUser = db.Users.Find(WebSecurity.GetUserId(User.Identity.Name));
            var model = new UserUpdateModel
            {
                About = currentUser.About,
                City = currentUser.City,
                Country = currentUser.Country,
                FullName = currentUser.FullName,
                PictureFilePath = currentUser.PictureFilePath
            };
            return View(model);
        }

        public JsonCamelCaseResult Current()
        {
            return new JsonCamelCaseResult(db.Users.Find(WebSecurity.GetUserId(User.Identity.Name)), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Library()
        {
            return View();
        }
    }
}
