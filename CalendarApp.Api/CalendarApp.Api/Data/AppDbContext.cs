using Microsoft.EntityFrameworkCore;
using CalendarApp.Models;
using System.Collections.Generic;

namespace CalendarApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}
