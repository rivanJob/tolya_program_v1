namespace MyWork2.Web.Models;

public class HistoryRecord
{
    public int Id { get; set; }
    public string? WHO { get; set; }
    public string? WHAT { get; set; }
    public string? FULLWHAT { get; set; }
    public DateTime? data { get; set; }
    public int? IDINCATALOG { get; set; }
}
