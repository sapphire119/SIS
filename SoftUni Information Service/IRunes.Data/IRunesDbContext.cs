namespace IRunes.Data
{
    using IRunes.Models;
    using Microsoft.EntityFrameworkCore;

    public class IRunesDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Album> Albums { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(ConnectionConfiguration.ConnectionString)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
