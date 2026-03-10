using Microsoft.AspNetCore.Mvc;
using MyWork2.Web.Models;
using MyWork2.Web.Services;

namespace MyWork2.Web.Controllers;

public class CatalogController : Controller
{
    private readonly IRepairOrderService _service;
    public CatalogController(IRepairOrderService service) => _service = service;

    public async Task<IActionResult> Index(string? search, bool includeClosed = true)
        => View(await _service.GetAllAsync(search, includeClosed));

    public async Task<IActionResult> Edit(int id)
        => View(await _service.GetByIdAsync(id) ?? new CatalogItem());

    [HttpPost]
    public async Task<IActionResult> Edit(CatalogItem item)
    {
        if (!ModelState.IsValid) return View(item);
        await _service.SaveAsync(item);
        return RedirectToAction(nameof(Index));
    }
}
