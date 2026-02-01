using API.Infrastructure.RequestDTOs.Users;
using API.Infrastructure.ResponseDTOs.Users;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

// UsersController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class UsersController : BaseCrudController<User, UserService, UserRequest, UserResponse>
{
    // Maps UserRequest DTO to User entity (for Create)
    protected override User MapToEntity(UserRequest request)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password  // Note: Password stored as plaintext for now
        };
    }
    
    // Maps User entity to UserResponse DTO (for all responses)
    // Password is excluded from response for security
    protected override UserResponse MapToResponse(User entity)
    {
        return new UserResponse
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email
            // Password NOT included in response
        };
    }
    
    // Updates existing User entity with values from UserRequest (for Update)
    protected override void UpdateEntity(User entity, UserRequest request)
    {
        entity.Username = request.Username;
        entity.Email = request.Email;
        entity.Password = request.Password;  // Note: Password stored as plaintext for now
    }
}
