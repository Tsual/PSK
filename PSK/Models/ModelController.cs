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



        public static void test()
        {
            Helper.RandomGenerator rso = new Helper.RandomGenerator();
           


            using (APPDbContext db = new APPDbContext())
            {
                if (db.Database.EnsureCreated())
                    db.Database.Migrate();

                StringSequenceObjA o1 = new StringSequenceObjA() { Data = rso.getRandomString(20) };
                db.Entry(o1).State = EntityState.Added;
                StringSequenceObjB o2 = new StringSequenceObjB() { Data = rso.getRandomString(20) };
                db.Entry(o2).State = EntityState.Added;
                User u = new User() { pid = "test", pwd = "TesT" };
                db.Entry(u).State = EntityState.Added;
                Recording obj = new Recording() { key = "fff", value = "fff", uid = 1 };
                db.Entry(obj).State = EntityState.Added;


                db.SaveChanges();


                var reslist1 = db.SA.ToList();
                var reslist2 = db.SB.ToList();
                var reslist3 = db.Users.ToList();
                var reslist4 = db.Recordings.ToList();
                int a = 0;
            }



        }

    }


    public class APPDbContext : DbContext
    {
        public DbSet<StringSequenceObjA> SA { get; set; }

        public DbSet<StringSequenceObjB> SB { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Recording> Recordings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        =>optionsBuilder.UseSqlite("FileName=efdb.db");
        
    }
    
}
