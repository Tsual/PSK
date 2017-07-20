using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    public class APPDbContext:DbContext
    {

    }

    public class UserDbContext : DbContext
    {

    }
}
