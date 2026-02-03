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
    private readonly UserService _userService;
    private readonly TokenServices _tokenServices;

    public AuthController(UserService userService, TokenServices tokenServices)
    {
        _userService = userService;
        _tokenServices = tokenServices;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userService.FindByCredentials(request.Username, request.Password);

        if (user == null)
            return Unauthorized("Invalid username or password");

        string token = _tokenServices.CreateToken(user);

        return Ok(token);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var existingUser = _userService.FindByUsernameOrEmail(request.Username, request.Email);

        if (existingUser != null)
            return BadRequest("Username or email already taken");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        _userService.Save(user);

        return Ok("User registered successfully");
    }
}
