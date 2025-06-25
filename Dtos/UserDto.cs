namespace Travidia.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public int RolId { get; set; }
    public int DepartmentId { get; set; }
    public int TitleId { get; set; }
    public int AreaId { get; set; }
    public bool Active { get; set; }
}