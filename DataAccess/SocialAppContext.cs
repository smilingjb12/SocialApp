using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using DataAccess.Configuration;

namespace DataAccess
{
    public class SocialAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }

        public static void Initialize()
        {
            Database.SetInitializer(new DatabaseInitializer());
            using (var c = new SocialAppContext())
            {
                c.Database.Initialize(force: true);
                c.Database.CreateIfNotExists();
            } 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new SongConfiguration());
        }
    }
}
