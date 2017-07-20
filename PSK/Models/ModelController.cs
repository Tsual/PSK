using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Context Model */
/*
public class testDbContext : DbContext
{
    public DbSet<testmodel> dats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("FileName=efdb.db");
    }
}
*/

namespace PSK.Models
{
    class ModelController
    {
        static List<DbContext> _contextList = new List<DbContext>()
        {

        };

        public static void Init()
        {
            foreach (var t in _contextList)
            {
                if (t.Database.EnsureCreated())
                {
                    throw new DbaseInitException(t);
                }
            }
        }

        public class DbaseInitException : Exception
        {
            public DbContext dat;
            public DbaseInitException(DbContext dat)
            {
                this.dat = dat;
                this.Source = dat.ToString();
            }
        }

    }


    public class APPDbContext : DbContext
    {
        public DbSet<StringSequence> SA { get; set; }

        public DbSet<StringSequenceB> SB { get; set; }

        public DbSet<User> Users { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=appdb.db");
        }
    }
    
    public class UserDbContext : DbContext
    {
        public DbSet<Data> Infos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=userdb.db");
        }
    }
}
