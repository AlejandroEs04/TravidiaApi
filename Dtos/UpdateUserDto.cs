namespace Travidia.Dtos;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public int RolId { get; set; }
    public int DepartmentId { get; set; }
    public string Password { get; set; } = "";
    public int SupervisorId { get; set; }
}