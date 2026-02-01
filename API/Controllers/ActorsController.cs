using API.Infrastructure.RequestDTOs.Actors;
using API.Infrastructure.ResponseDTOs.Actors;
using Common.Entities;
using Common.Services;

namespace API.Controllers;

// ActorController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class ActorsController : BaseCrudController<Actor, ActorService, ActorRequest, ActorResponse>
{
    // Maps ActorRequest DTO to Actor entity (for Create)
    protected override Actor MapToEntity(ActorRequest request)
    {
        return new Actor
        {
            Name = request.Name
        };
    }
    
    // Maps Actor entity to ActorResponse DTO (for all responses)
    protected override ActorResponse MapToResponse(Actor entity)
    {
        return new ActorResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    // Updates existing Actor entity with values from ActorRequest (for Update)
    protected override void UpdateEntity(Actor entity, ActorRequest request)
    {
        entity.Name = request.Name;
    }
}
