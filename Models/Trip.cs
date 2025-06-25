namespace Travidia.Models;

public class Trip
{
    public int Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnedDate { get; set; }
    public string Destiny { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Purpose { get; set; } = "";
    public int RequestId { get; set; }
}