namespace MyWork2.Web.Models;

public class StockItem
{
    public int Id { get; set; }
    public string? Naimenovanie { get; set; }
    public string? Kategoriya { get; set; }
    public string? Podkategoriya { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? CountOf { get; set; }
    public decimal? Price { get; set; }
    public string? Primechanie { get; set; }
}
