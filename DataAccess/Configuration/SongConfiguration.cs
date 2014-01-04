using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace DataAccess.Configuration
{
    class SongConfiguration : EntityTypeConfiguration<Song>
    {
        public SongConfiguration()
        {
            HasRequired(s => s.Uploader).WithMany(u => u.UploadedSongs).HasForeignKey(s => s.UploaderId);
        }
    }
}
