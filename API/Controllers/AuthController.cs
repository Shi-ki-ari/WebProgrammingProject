using System.Linq;
using API.Infrastructure.RequestDTOs.Users;
using API.Services;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Common.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService userService;
    private readonly TokenServices tokenService;

    public AuthController(UserService userServiceInjection, TokenServices tokenServiceInjection)
    {
        userService = userServiceInjection;
        tokenService = tokenServiceInjection;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = userService.FindByUsername(request.Username);

        if (user == null || !userService.VerifyPassword(request.Password, user.Password))
            return Unauthorized("Invalid username or password");

        string token = tokenService.CreateToken(user);

        return Ok(token);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var existingUser = userService.FindByUsernameOrEmail(request.Username, request.Email);

        if (existingUser != null)
            return BadRequest("Username or email already taken");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = userService.HashPassword(request.Password)
        };

        userService.Save(user);

        return Ok("User registered successfully");
    }
}
