using Microsoft.AspNetCore.Mvc;
using MyWork2.Web.Services;

namespace MyWork2.Web.Controllers;

public class StockController : Controller
{
    private readonly IStockService _service;
    public StockController(IStockService service) => _service = service;

    public async Task<IActionResult> Index(string? search)
        => View(await _service.GetAllAsync(search));
}
