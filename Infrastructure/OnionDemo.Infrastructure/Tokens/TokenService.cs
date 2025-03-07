﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnionDemo.Application.Interfaces.Tokens;
using OnionDemo.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnionDemo.Infrastructure.Tokens;

public class TokenService : ITokenService
{
    private readonly TokenSettings _tokenSettings;
    private readonly UserManager<User> _userManager;

    public TokenService(IOptions<TokenSettings> options, UserManager<User> userManager)
    {
        _tokenSettings = options.Value;
        _userManager = userManager;
    }

    public async Task<JwtSecurityToken> CreateToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        foreach (var role in roles)
            claims.Add(new(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));
        var token = new JwtSecurityToken(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenSettings.TokenValidityInMinutes),
            signingCredentials: new(key, SecurityAlgorithms.HmacSha256)
        );

        await  _userManager.AddClaimsAsync(user, claims);

        return token;
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken()
    {
        throw new NotImplementedException();
    }
}