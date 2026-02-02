using API.Infrastructure.RequestDTOs.Users;
using API.Infrastructure.ResponseDTOs.Users;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseCrudController<User, UserService, UserRequest, UserResponse>
{
    public UsersController(UserService service) : base(service)
    {
    }

    protected override User MapToEntity(UserRequest request)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
    }
    

    protected override UserResponse MapToResponse(User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email
        };
    }
    
    protected override void UpdateEntity(User entity, UserRequest request)
    {
        entity.Username = request.Username;
        entity.Email = request.Email;
        entity.Password = request.Password;
    }
}
