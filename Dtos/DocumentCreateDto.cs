namespace Travidia.Dtos;

public class DocumentCreateDto
{
    public string Name { get; set; } = "";
    public IEnumerable<ApprovalStepCreateDto> Steps { get; set; } = [];
}