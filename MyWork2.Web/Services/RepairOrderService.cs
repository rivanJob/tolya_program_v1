using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Data;
using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public class RepairOrderService : IRepairOrderService
{
    private readonly AppDbContext _db;
    public RepairOrderService(AppDbContext db) => _db = db;

    public async Task<List<CatalogItem>> GetAllAsync(string? search, bool includeClosed)
    {
        var query = _db.Catalog.AsQueryable().Where(x => !x.Deleted);
        if (!includeClosed)
            query = query.Where(x => x.Data_vidachi == null);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.surname!.Contains(search) || x.phone!.Contains(search));

        return await query.OrderByDescending(x => x.Id).Take(500).ToListAsync();
    }

    public Task<CatalogItem?> GetByIdAsync(int id) => _db.Catalog.FirstOrDefaultAsync(x => x.Id == id);

    public async Task SaveAsync(CatalogItem item)
    {
        if (item.Id == 0) _db.Catalog.Add(item);
        else _db.Catalog.Update(item);
        await _db.SaveChangesAsync();
    }
}
