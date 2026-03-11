namespace MyWork2.Web.Models;

public class User
{
    public int Id { get; set; }
    public string type { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int id_gruppi_dostupa { get; set; }
    public string user_pwd { get; set; } = string.Empty;
}
