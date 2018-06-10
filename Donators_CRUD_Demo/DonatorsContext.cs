using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donators_CRUD_Demo
{
    class DonatorsContext:DbContext
    {
        public DonatorsContext()
            : base("name=DonatorsConn")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DonatorsContext>());
        }

        public DbSet<Donator> Donators { get; set; }
        public DbSet<Province> Provinces { get; set; }
    }
}
