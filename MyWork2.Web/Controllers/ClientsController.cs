using Microsoft.AspNetCore.Mvc;
using MyWork2.Web.Services;

namespace MyWork2.Web.Controllers;

public class ClientsController : Controller
{
    private readonly IClientService _service;
    public ClientsController(IClientService service) => _service = service;

    public async Task<IActionResult> Index(string? search)
        => View(await _service.GetAllAsync(search));
}
