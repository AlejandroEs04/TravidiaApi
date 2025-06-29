namespace Travidia.Dtos;

public class ApprovalStepCreateDto
{
    public int TitleId { get; set; }
    public int? DepartmentId { get; set; }
    public int? AreaId { get; set; }
    public int Order { get; set; }
    public int DocumentId { get; set; }
}