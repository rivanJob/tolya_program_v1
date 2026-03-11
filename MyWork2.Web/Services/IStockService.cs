using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public interface IStockService
{
    Task<List<StockItem>> GetAllAsync(string? search);
}
