namespace Travidia.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public int RolId { get; set; }
    public int DepartmentId { get; set; }
    public int AreaId { get; set; }
    public int TitleId { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public int SupervisorId { get; set; }

    private readonly DataContextDapper? _dapper;

    public User() { }

    public User(DataContextDapper? dapper = null)
    {
        _dapper = dapper;
    }

    public User? GetUserById(int id)
    {
        if (_dapper is null)
            throw new NullReferenceException("Dapper is not available");

        string queryUser = "SELECT * FROM [User] WHERE id = @id";
        var user = _dapper.QuerySingle<User>(queryUser, new { id });
        return user;
    }

    public static bool IsUserOwner(int userId, int originatorId) => userId == originatorId;
}