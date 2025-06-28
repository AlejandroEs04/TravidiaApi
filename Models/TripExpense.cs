namespace Travidia.Models;

public class TripExpense
{
    public int Id { get; set; }
    public int TripRequestId { get; set; }
    public int RequestId { get; set; }
    public int ExpenseId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}