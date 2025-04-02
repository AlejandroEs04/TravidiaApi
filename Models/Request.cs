using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Travidia.Models;

public class Request
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int RequestStepId { get; set; }
    public bool? Response { get; set; }
    public int? DepartmentId { get; set; }
    public int? RolId { get; set; }
    public int? UserId { get; set; }
    public DateTime? ResponsedAt { get; set; }
}