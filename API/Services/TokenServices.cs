using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Entities;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenServices
{
    public string CreateToken(User user)
    {
        Claim[] claims = new Claim[]
        {
            new Claim("loggedUserId", user.Id.ToString()),
            new Claim("role", user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("!Password123!Password123!Password123"));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "fmi",
            audience: "front-end",
            claims: claims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: cred
        );
        string tokenData = new JwtSecurityTokenHandler()
                                            .WriteToken(token);
        
        return tokenData;
    }
}
