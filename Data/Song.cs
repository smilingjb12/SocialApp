using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Song
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        public string FilePath { get; set; }
        public int LengthInSeconds { get; set; }
        public int Likes { get; set; }

        public int UploaderId { get; set; }
        public virtual User Uploader { get; set; }
    }
}
