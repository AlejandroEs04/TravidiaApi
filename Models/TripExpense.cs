namespace Travidia.Models;

public class TripExpense
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public int ExpenseTypeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ReportedAt { get; set; }
}