using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Models;
using Microsoft.IdentityModel.Tokens;
namespace Auth.Services;

public class TokenGenerator
{
    public string Generate(User user)
    {
        var Secret = "4d82a63bbdc67c1e4784ed6587f3730c";
        var ExpiresHours = 4;
        var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = AddClaims(user),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret)),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.Now.AddHours(ExpiresHours)
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