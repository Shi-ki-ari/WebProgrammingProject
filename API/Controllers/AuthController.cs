using System.Linq;
using API.Infrastructure.RequestDTOs.Users;
using API.Services;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private UserService _userService = new UserService();

    // POST: api/auth/login
    // Authenticates a user with username and password and returns JWT token
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Find user with matching username and password
        var user = _userService.GetAll()
            .FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        // Generate JWT token
        TokenServices tokenService = new TokenServices();
        string token = tokenService.CreateToken(user);

        // Return token
        return Ok(new
        {
            token = token
        });
    }
}
