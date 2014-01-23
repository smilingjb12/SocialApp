using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data;
using SocialApp.Models;
using System.Data.Entity;

namespace SocialApp.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        public ActionResult Show(int id)
        {
            User user = Db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Update(UserUpdateModel model)
        {
            User currentUser = Db.Users.Find(CurrentUserId);
            if (model.Picture != null)
            {
                string extension = System.IO.Path.GetExtension(model.Picture.FileName);
                string relativePicturePath = string.Format("/{0}/{1}{2}", Strings.ProfilePicturesFolder, GenerateFileName(), extension);
                string serverPicturePath = Server.MapPath(string.Format("~/{0}", relativePicturePath));
                model.Picture.SaveAs(serverPicturePath);
                currentUser.PictureFilePath = relativePicturePath;
            }
            // TODO: use automapper here
            currentUser.About = model.About;
            currentUser.City = model.City;
            currentUser.Country = model.Country;
            currentUser.FullName = model.FullName;
            Db.SaveChanges();

            return RedirectToAction("Settings");
        }

        public ViewResult Settings()
        {
            User currentUser = Db.Users.Find(CurrentUserId);
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

        public JsonCamelCaseResult UploadedSongs()
        {
            IEnumerable<Song> songs = Db.Songs
                .Include(song => song.Tags)
                .Where(song => song.UploaderId == CurrentUserId)
                .ToList();

            return new JsonCamelCaseResult(songs, JsonRequestBehavior.AllowGet);
        }

        public JsonCamelCaseResult Current()
        {
            return new JsonCamelCaseResult(Db.Users.Find(CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Library()
        {
            return View();
        }
    }
}
