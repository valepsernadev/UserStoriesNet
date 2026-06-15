using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmeza.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
  UserManager<IdentityUser> userManager,
  IConfiguration config) : ControllerBase
{
  /// <summary>Registra un nuevo usuario con rol Cliente.</summary>
  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto dto)
  {
    var user = new IdentityUser
    {
      UserName = dto.Email,
      Email = dto.Email,
      EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(user, dto.Password);

    if (!result.Succeeded)
      return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

    await userManager.AddToRoleAsync(user, "Cliente");

    return Ok(new { message = "Usuario registrado correctamente." });
  }

  /// <summary>Autentica credenciales y devuelve un JWT.</summary>
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginDto dto)
  {
    var user = await userManager.FindByEmailAsync(dto.Email);

    if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
      return Unauthorized(new { message = "Credenciales inválidas." });

    var roles = await userManager.GetRolesAsync(user);
    var role = roles.FirstOrDefault() ?? "Cliente";

    return Ok(BuildToken(user, role));
  }

  private AuthResponseDto BuildToken(IdentityUser user, string role)
  {
    var expiration = DateTime.UtcNow.AddHours(8);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email!),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email!),
      new Claim(ClaimTypes.Role, role),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: config["Jwt:Issuer"],
      audience: config["Jwt:Audience"],
      claims: claims,
      expires: expiration,
      signingCredentials: creds);

    return new AuthResponseDto
    {
      Token = new JwtSecurityTokenHandler().WriteToken(token),
      Email = user.Email!,
      Role = role,
      Expiration = expiration
    };
  }
}
