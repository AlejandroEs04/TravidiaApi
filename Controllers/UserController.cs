using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Travidia.Dtos;
using Travidia.Models;
using Travidia.Utils;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new (config);
    private readonly AuthHelper _authHelper = new (config);

    [HttpGet]
    public IEnumerable<User> GetUsers()
    {
        var users = _dapper.Query<User>("SELECT * FROM [User]");
        return users;
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserDto user)
    {
        try
        {
            byte[] passwordSalt = new byte[129/8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);

            var parameters = new {
                @fullName = user.FullName,
                @email = user.Email,
                @rolId = user.RolId,
                @departmentId = user.DepartmentId,
                passwordHash,
                passwordSalt
            };

            _dapper.Execute("Spu_UserUpset", parameters);

            return Ok();
        }
        catch (Exception)
        {
            throw;
        }
    }
}