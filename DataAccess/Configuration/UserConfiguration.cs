﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace DataAccess.Configuration
{
    class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            HasMany(u => u.UploadedSongs).WithRequired(s => s.Uploader).HasForeignKey(s => s.UploaderId);
            HasMany(u => u.LibrarySongs).WithOptional();
        }
    }
}
