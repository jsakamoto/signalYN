using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SignalYN.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public AppDbContext()
        {
#if DEBUG
            this.Database.Log = log => System.Diagnostics.Debug.WriteLine(log);
#endif
        }
    }
}