namespace Travidia.Models;

public class ApprovalStep
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public int TitleId { get; set; }
    public int? DepartmentId { get; set; }
    public int? AreaId { get; set; }
    public int Order { get; set; }
}