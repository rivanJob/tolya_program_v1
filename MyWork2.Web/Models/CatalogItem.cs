namespace MyWork2.Web.Models;

public class CatalogItem
{
    public int Id { get; set; }
    public DateTime? Data_priema { get; set; }
    public DateTime? Data_vidachi { get; set; }
    public string? surname { get; set; }
    public string? phone { get; set; }
    public string? WhatRemont { get; set; }
    public string? brand { get; set; }
    public string? model { get; set; }
    public string? SerialNumber { get; set; }
    public string? polomka { get; set; }
    public decimal? predvaritelnaya_stoimost { get; set; }
    public string? Status_remonta { get; set; }
    public string? master { get; set; }
    public int? ClientId { get; set; }
    public bool Deleted { get; set; }
}
