using ClientConsole.Model;
using Microsoft.EntityFrameworkCore;

namespace ClientConsole.Persistence
{
    public class AppDbContext: DbContext
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Barcode> Barcodes { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>()
            .HasMany(m => m.Barcodes)
            .WithOne(b => b.MenuItem)
            .HasForeignKey(b => b.MenuItemServerId)
            .HasPrincipalKey(m => m.ServerId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
