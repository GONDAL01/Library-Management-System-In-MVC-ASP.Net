using Project5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project5.Data
{
    public class Project5Context : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public Project5Context() : base("name=Project5Context")
        {
        }

        public System.Data.Entity.DbSet<Project5.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<Project5.Models.Role> Roles { get; set; }

        public System.Data.Entity.DbSet<Project5.Models.UserRole> UserRoles { get; set; }

        public System.Data.Entity.DbSet<Project5.Models.Book> Books { get; set; }

        public System.Data.Entity.DbSet<Project5.Models.Borrowing> Borrowings { get; set; }
        public DbSet<BookReturn> BookReturns { get; set; }


    }
}
