namespace Travidia.Dtos;

public class TripCreateDto
{
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnedDate { get; set; }
    public string Destiny { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Purpose { get; set; } = "";
}