using API.Infrastructure.RequestDTOs.Actors;
using API.Infrastructure.ResponseDTOs.Actors;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class ActorsController : BaseCrudController<Actor, ActorService, ActorRequest, ActorResponse>
{
    public ActorsController(ActorService service) : base(service)
    {
    }

    protected override Actor MapToEntity(ActorRequest request)
    {
        return new Actor
        {
            Name = request.Name
        };
    }
    
    protected override ActorResponse MapToResponse(Actor entity)
    {
        return new ActorResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    protected override void UpdateEntity(Actor entity, ActorRequest request)
    {
        entity.Name = request.Name;
    }
}
