using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace Travidia.Utils 
{
    public class AuthHelper(IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password : password, 
                salt : Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf : KeyDerivationPrf.HMACSHA256, 
                iterationCount : 1000000, 
                numBytesRequested : 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            Claim[] claims =
            [
                new("userId", userId.ToString())
            ];

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new(
                Encoding.UTF8.GetBytes(
                    tokenKeyString ?? ""
                )
            );

            SigningCredentials credentials = new(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new()
                {
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = credentials, 
                    Expires = DateTime.Now.AddDays(90)
                };

            JwtSecurityTokenHandler tokenHandler = new();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}