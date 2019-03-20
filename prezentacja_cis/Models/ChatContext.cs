using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prezentacja_cis.Models
{
    public class ChatContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./chat.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany(x => x.Messsages)
                .WithOne(X => X.Room)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
