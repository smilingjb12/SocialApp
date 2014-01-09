using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using DataAccess;
using SocialApp.Models;
using TagLib;
using WebMatrix.WebData;

namespace SocialApp.Controllers
{
    public class SongController : Controller
    {
        private const string SongDirectory = "/Content/Uploads/Songs/";
        private const string AlbumCoverDirectory = "/Content/Uploads/AlbumCovers/";

        private readonly SocialAppContext db;

        public SongController(SocialAppContext db)
        {
            this.db = db;
        }
        
        [HttpPost]
        public JsonResult Update(Song song)
        {
            Song existingSong = db.Songs.Find(song.Id);
            if (existingSong == null) return null;

            existingSong.Title = song.Title;
            existingSong.Artist = song.Artist;
            existingSong.Album = song.Album;

            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonCamelCaseResult Upload()
        {
            byte[] songBytes = FileUtils.ReadBytesFromStream(Request.InputStream);
            string fileName = string.Format("{0}.mp3", Guid.NewGuid());
            string songServerDirectory = Server.MapPath("~" + SongDirectory);
            if (!Directory.Exists(songServerDirectory))
            {
                Directory.CreateDirectory(songServerDirectory);
            }
            string path = string.Format("{0}{1}", SongDirectory, fileName);
            System.IO.File.WriteAllBytes(Server.MapPath("~" + path), songBytes);

            Song song = new Song
            {
                UploaderId = WebSecurity.GetUserId(User.Identity.Name),
                FilePath = path,
                FileSizeInMegaBytes = Math.Round(Request.ContentLength / 1024d / 1024d, 2)
            };

            string albumCoverDirectory = Server.MapPath("~" + AlbumCoverDirectory);
            if (!Directory.Exists(albumCoverDirectory))
            {
                Directory.CreateDirectory(albumCoverDirectory);
            }

            using (TagLib.File tagFile = TagLib.File.Create(Server.MapPath("~" + path)))
            {
                song.Bitrate = tagFile.Properties.AudioBitrate;
                song.Duration = tagFile.Properties.Duration;
                song.Title = tagFile.Tag.Title;
                song.Artist = tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstArtist;
                song.Album = tagFile.Tag.Album;
                if (tagFile.Tag.Pictures.FirstOrDefault() != null) // has album cover
                {
                    IPicture pic = tagFile.Tag.Pictures.First();
                    string extension = pic.MimeType.Substring(pic.MimeType.LastIndexOf('/') + 1);
                    string picFileName = string.Format("{0}.{1}", Guid.NewGuid(), extension);
                    string picPath = string.Format("{0}{1}", AlbumCoverDirectory, picFileName);
                    System.IO.File.WriteAllBytes(Server.MapPath("~" + picPath), pic.Data.ToArray());
                    song.AlbumCoverPicturePath = picPath;
                }
                else
                {
                    song.AlbumCoverPicturePath = "/Content/images/default-album-cover.gif";
                }
            }
            db.Songs.Add(song);
            db.SaveChanges();

            return new JsonCamelCaseResult(song, JsonRequestBehavior.DenyGet);
        }

    }
}
