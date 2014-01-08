using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace DataAccess
{
    internal class DatabaseInitializer : DropCreateDatabaseIfModelChanges<SocialAppContext>
    {
        protected override void Seed(SocialAppContext context)
        {
            User user = new User
            {
                IsActivated = true,
                Email = "mail@mail.mail",
                Password = "1234"
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
