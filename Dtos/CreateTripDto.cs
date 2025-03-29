namespace Travidia.Dtos;

public class CreateTripDto
{
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Destiny { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Purpose { get; set; } = "";
}