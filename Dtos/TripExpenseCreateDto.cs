namespace Travidia.Dtos;

public class TripExpenseCreateDto
{
    public int ExpenseId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ReportedDate { get; set; }
}