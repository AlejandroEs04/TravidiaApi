namespace Travidia.Models;

public class Request
{
    public int Id { get; set; }
    public int OriginatorId { get; set; }
    public int DocumentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int StatusId { get; set; }
}