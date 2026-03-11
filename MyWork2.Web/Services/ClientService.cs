using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Data;
using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _db;
    public ClientService(AppDbContext db) => _db = db;

    public async Task<List<Client>> GetAllAsync(string? search)
    {
        var query = _db.Clients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.FIO!.Contains(search) || x.Phone!.Contains(search));
        return await query.OrderByDescending(x => x.Id).Take(500).ToListAsync();
    }
}
