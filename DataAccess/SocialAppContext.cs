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

        static SocialAppContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SocialAppContext>());
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
