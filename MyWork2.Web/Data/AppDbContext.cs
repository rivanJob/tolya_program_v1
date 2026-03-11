using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Models;

namespace MyWork2.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CatalogItem> Catalog => Set<CatalogItem>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<StockItem> Stock => Set<StockItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AccessGroup> AccessGroups => Set<AccessGroup>();
    public DbSet<HistoryRecord> History => Set<HistoryRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogItem>().ToTable("catalog");
        modelBuilder.Entity<Client>().ToTable("clientsmap");
        modelBuilder.Entity<StockItem>().ToTable("stock");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<AccessGroup>().ToTable("groupdostup");
        modelBuilder.Entity<HistoryRecord>().ToTable("historybd");
    }
}
