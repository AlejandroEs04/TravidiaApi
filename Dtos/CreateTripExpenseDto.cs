namespace Travidia.Dtos;

public class CreateTripExpenseDto 
{
    public int TripId { get; set; }
    public int ExpenseTypeId { get; set; }
    public decimal Amount { get; set; }
}