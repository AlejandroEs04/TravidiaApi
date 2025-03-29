namespace Travidia.Dtos;

public class CreateUserDto 
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public int RolId { get; set; }
    public int DepartmentId { get; set; }
    public string Password { get; set; } = "";
}