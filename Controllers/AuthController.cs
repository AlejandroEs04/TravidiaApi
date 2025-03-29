using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;
using Travidia.Utils;

namespace Travidia.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);
    private readonly AuthHelper _authHelper = new(config);

    [HttpPost]
    public IActionResult Login(AuthDto auth)
    {
        try
        {
            string existsUser = "SELECT * FROM [User] WHERE email = @email";

            var userExists = _dapper.QuerySingle<User>(existsUser, new { @email = auth.Email });
                
            if(userExists == null) return StatusCode(401, "User email doesn't exists");

            byte[] passwordHash = _authHelper.GetPasswordHash(auth.Password, userExists.PasswordSalt);

            for(int i=0; i<passwordHash.Length;i++) {
                if(passwordHash[i] != userExists.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect Password");
                }
            }

            return Ok(new Dictionary<string, string> {{"token", _authHelper.CreateToken(userExists.Id)}});
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";

        string sqlGetUserId = "SELECT * FROM [User] WHERE id = @id";

        int userIdFromDB = _dapper.QuerySingle<int>(sqlGetUserId, new { @id = userId });

        return Ok(new Dictionary<string, string> {
            {"token", _authHelper.CreateToken(userIdFromDB)}
        });

    }
}