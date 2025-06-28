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
public class UserController(IConfiguration config, DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;
    private readonly AuthHelper _authHelper = new (config);

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _dapper.Query<UserDto>("SELECT * FROM [User]");
        return Ok(users);
    }

    [AllowAnonymous]
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
                @name = user.Name,
                @lastName = user.LastName,
                @email = user.Email,
                @rolId = user.RolId,
                @departmentId = user.DepartmentId,
                @titleId = user.TitleId,
                @areaId = user.AreaId,
                passwordHash,
                passwordSalt
            };

            _dapper.Execute("User_upsert", parameters);

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
                @name = user.Name,
                @lastName = user.LastName,
                @email = user.Email,
                @rolId = user.RolId,
                @departmentId = user.DepartmentId,
                @titleId = user.TitleId,
                @areaId = user.AreaId,
            };

            _dapper.Execute("User_upsert", parameters);

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

            _dapper.Execute("User_upsert", parameters);
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

        _dapper.Execute("User_upsert", parameters);
    }
}