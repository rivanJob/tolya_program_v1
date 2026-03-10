using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public interface IRepairOrderService
{
    Task<List<CatalogItem>> GetAllAsync(string? search, bool includeClosed);
    Task<CatalogItem?> GetByIdAsync(int id);
    Task SaveAsync(CatalogItem item);
}
