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

    // POST: api/auth/register
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Check if username or email already exists
        var existingUser = _userService.GetAll()
            .FirstOrDefault(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
            return BadRequest("Username or email already taken");

        // Create new user
        var user = new Common.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        _userService.Save(user);

        return Ok("User registered successfully");
    }
}
