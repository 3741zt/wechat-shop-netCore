using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.EntityFramework.Data
{
    public class DbInitializer
    {
        public static void Initalize(DemoDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.UserApp.Any())
            {
                return;
           }
            context.SaveChanges();


        }
    }
}
