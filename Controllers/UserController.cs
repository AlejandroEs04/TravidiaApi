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
    public IEnumerable<UserDto> GetUsers()
    {
        var users = _dapper.Query<UserDto>("SELECT * FROM [User]");
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
                passwordSalt, 
                @supervisorId = user.SupervisorId
            };

            _dapper.Execute("Spu_UserUpsert", parameters);

            return Ok();
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{userId}")]
    public IActionResult UpdateUser(int userId, UpdateUserDto user)
    {
        try
        {
            var parameters = new 
            {
                @id = userId,
                @fullName = user.FullName,
                @email = user.Email,
                @rolId = user.RolId,
                @departmentId = user.DepartmentId, 
                @supervisorId = user.SupervisorId
            };

            _dapper.Execute("Spu_UserUpsert", parameters);

            if(user.Password.Length > 0) UpdatePassword(user.Password, userId);
        }
        catch (Exception)
        {
            throw;
        }

        return Ok();
    }

    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        try
        {
            var parameters = new 
            {
                @id = userId,
                @active = false
            };

            _dapper.Execute("Spu_UserUpsert", parameters);
        }
        catch (Exception)
        {
            throw;
        }

        return Ok();
    }

    protected void UpdatePassword(string password, int userId)
    {
        byte[] passwordSalt = new byte[129/8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            rng.GetNonZeroBytes(passwordSalt);

        byte[] passwordHash = _authHelper.GetPasswordHash(password, passwordSalt);

        var parameters = new 
        {
            @id = userId,
            passwordHash, 
            passwordSalt
        };  

        _dapper.Execute("Spu_UserUpsert", parameters);
    }
}