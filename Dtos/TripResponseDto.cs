namespace Travidia.Models;

public class TripResponseDto
{
    public int Id { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnedDate { get; set; }
    public string Destiny { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Purpose { get; set; } = "";
    public int RequestId { get; set; }
    public string OriginatorName { get; set; } = "";
    public string Document { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; } = "";
}