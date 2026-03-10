using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Data;
using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _db;
    public StockService(AppDbContext db) => _db = db;

    public async Task<List<StockItem>> GetAllAsync(string? search)
    {
        var query = _db.Stock.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Naimenovanie!.Contains(search) || x.Brand!.Contains(search));
        return await query.OrderByDescending(x => x.Id).Take(500).ToListAsync();
    }
}
