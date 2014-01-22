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
    public class SongController : BaseController
    {
        private const string SongDirectory = "/Content/Uploads/Songs/";
        private const string AlbumCoverDirectory = "/Content/Uploads/AlbumCovers/";

        [HttpPost]
        public JsonResult Update(Song song)
        {
            Song existingSong = Db.Songs.Find(song.Id);
            if (existingSong == null) return null;

            existingSong.Title = song.Title;
            existingSong.Artist = song.Artist;
            existingSong.Album = song.Album;

            Db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            Song song = Db.Songs.Find(id);
            Db.Songs.Remove(song);
            Db.SaveChanges();
            return Json(string.Empty);
        }

        public FileResult Download(int id)
        {
            Song song = Db.Songs.Find(id);
            if (song == null) return null;
            string path = Server.MapPath("~" + song.FilePath);
            var fileStream = new FileStream(path, FileMode.Open);
            return File(fileStream, "audio/mp3");
        }

        [HttpPost]
        public JsonCamelCaseResult Upload()
        {
            byte[] songBytes = FileUtils.ReadBytesFromStream(Request.InputStream);
            string fileName = string.Format("{0}.mp3", GenerateFileName());
            string songServerDirectory = Server.MapPath("~" + SongDirectory);
            if (!Directory.Exists(songServerDirectory))
            {
                Directory.CreateDirectory(songServerDirectory);
            }
            string path = string.Format("{0}{1}", SongDirectory, fileName);
            System.IO.File.WriteAllBytes(Server.MapPath("~" + path), songBytes);

            Song song = new Song
            {
                UploaderId = CurrentUserId,
                FilePath = path,
                FileSizeInMegaBytes = Math.Round(Request.ContentLength / 1024d / 1024d, 2),
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
                song.Title = tagFile.Tag.Title ?? "Song Title";
                song.Artist = tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstArtist;
                song.Artist = song.Artist ?? "Song Artist";
                song.Album = tagFile.Tag.Album ?? "Song Album Title";
                if (tagFile.Tag.Pictures.FirstOrDefault() != null) // has album cover
                {
                    IPicture pic = tagFile.Tag.Pictures.First();
                    string extension = pic.MimeType.Substring(pic.MimeType.LastIndexOf('/') + 1);
                    string picFileName = string.Format("{0}.{1}", GenerateFileName(), extension);
                    string picPath = string.Format("{0}{1}", AlbumCoverDirectory, picFileName);
                    System.IO.File.WriteAllBytes(Server.MapPath("~" + picPath), pic.Data.ToArray());
                    song.AlbumCoverPicturePath = picPath;
                }
                else
                {
                    song.AlbumCoverPicturePath = "/Content/images/default-album-cover.gif";
                }
            }
            Db.Songs.Add(song);
            Db.SaveChanges();

            return new JsonCamelCaseResult(song, JsonRequestBehavior.DenyGet);
        }

    }
}
