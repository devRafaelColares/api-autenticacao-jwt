using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Models;
using Microsoft.IdentityModel.Tokens;
namespace Auth.Services;

public class TokenGenerator
{
    private readonly string _secret;
    private readonly int _expiresHours;

    public TokenGenerator(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret");
        _expiresHours = int.Parse(configuration["Jwt:ExpiresHours"] ?? "1");
    }

    public string Generate(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = AddClaims(user),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret)),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = DateTime.UtcNow.AddHours(_expiresHours)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsIdentity AddClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Role, user.Access!));
        claims.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Name!));
        return claims;
    }
}
