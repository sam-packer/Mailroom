using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mailroom.Models;
using Microsoft.IdentityModel.Tokens;

namespace Mailroom.Jwt;

public class JwtHelper
{
    public static string GenerateToken(User user, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.First_Name + " " + user.Last_Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(double.Parse(jwtConfig["ExpireHours"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string? FetchToken(HttpRequest httpRequest)
    {
        return FetchToken(httpRequest.HttpContext);
    }

    public static string? FetchToken(HttpContext httpContext)
    {
        return httpContext.Request.Cookies.TryGetValue("AuthToken", out var tokenValue)
            ? tokenValue
            : null;
    }

    public static string? FetchUserClaim(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return null;
        }

        var jwtHandler = new JwtSecurityTokenHandler();
        if (!jwtHandler.CanReadToken(jwtToken)) return null;
        var token = jwtHandler.ReadJwtToken(jwtToken);
        var userClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        return userClaim?.Value;
    }

    public static string? GetNameFromJwt(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken)) return null;
        var token = handler.ReadJwtToken(jwtToken);
        var roleClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
        return roleClaim?.Value;
    }

    public static string? GetRoleFromJwt(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken)) return null;
        var token = handler.ReadJwtToken(jwtToken);
        var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        return roleClaim?.Value;
    }
}