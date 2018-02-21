using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.DAL;

namespace UserManagement.BL
{
    public static class TokenManager
    {
        /// <summary>
        /// Creates access token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string CreateAccessToken(User user)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWTKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };

            var jwtIssuer = Environment.GetEnvironmentVariable("JWTIssuer");
            var jwtAudience = Environment.GetEnvironmentVariable("JWTAudience");
            var jwtExp = Environment.GetEnvironmentVariable("JWTExpInMin");
            Int32.TryParse(jwtExp, out var jwtExpAfter);

            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: DateTime.Now.AddMinutes(jwtExpAfter),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
