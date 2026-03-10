using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWork2.Web.Data;
using MyWork2.Web.ViewModels;

namespace MyWork2.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new HomeIndexVm
        {
            TotalOrders = await _db.Catalog.CountAsync(),
            OpenOrders = await _db.Catalog.CountAsync(x => x.Data_vidachi == null && !x.Deleted),
            Clients = await _db.Clients.CountAsync(),
            StockItems = await _db.Stock.CountAsync()
        };
        return View(vm);
    }

    public IActionResult LegacyModules() => View();
}
