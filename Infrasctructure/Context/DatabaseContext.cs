using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using System.IO;

namespace SCED.API.Infrasctructure.Context
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceData> DeviceData { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Shelter> Shelters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
