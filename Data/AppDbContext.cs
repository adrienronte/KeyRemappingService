using KeyRemappingService.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyRemappingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Keyboard> Keyboards { get; set; }
        public DbSet<KeyMapping> KeyMappings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyMapping>()
                .HasIndex(m => new
                {
                    m.KeyboardId,
                    m.SourceKeyCode
                })
                .IsUnique();
        }
    }
}