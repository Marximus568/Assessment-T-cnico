using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await GenerateTokenAsync(user);
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var secretKey = _configuration["JwtSettings:Secret"] 
                        ?? _configuration["JWTSETTINGS__SECRET"]
                        ?? Environment.GetEnvironmentVariable("JWTSETTINGS__SECRET")
                        ?? throw new InvalidOperationException("JWT Secret not configured. Ensure JWTSETTINGS__SECRET is set in .env");
        
        var issuer = _configuration["JwtSettings:Issuer"] 
                     ?? _configuration["JWTSETTINGS__ISSUER"]
                     ?? Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER");

        var audience = _configuration["JwtSettings:Audience"] 
                       ?? _configuration["JWTSETTINGS__AUDIENCE"]
                       ?? Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationMinutesStr = _configuration["JwtSettings:ExpirationMinutes"] 
                                  ?? _configuration["JWTSETTINGS__EXPIRATIONMINUTES"]
                                  ?? Environment.GetEnvironmentVariable("JWTSETTINGS__EXPIRATIONMINUTES")
                                  ?? "60";
        
        var expirationMinutes = int.Parse(expirationMinutesStr);
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.FullName))
            claims.Add(new Claim(ClaimTypes.Name, user.FullName));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Email = user.Email!,
            ExpiresAt = expiresAt
        };
    }
}
