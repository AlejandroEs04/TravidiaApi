namespace Travidia.Models;

public class RequestStep 
{
    public int Id { get; set; }
    public int RequestFlowId { get; set; }
    public bool IsFinish { get; set; }
    public int NextStepId { get; set; } 
    public int DepartmentId { get; set; }
    public int RolId { get; set; }
    public int UserId { get; set; }
}