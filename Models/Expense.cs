namespace Travidia.Models;

public class Expense
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal? Limit { get; set; }
}