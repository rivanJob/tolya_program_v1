using MyWork2.Web.Models;

namespace MyWork2.Web.Services;

public interface IClientService
{
    Task<List<Client>> GetAllAsync(string? search);
}
