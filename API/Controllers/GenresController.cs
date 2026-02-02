using API.Infrastructure.RequestDTOs.Genres;
using API.Infrastructure.ResponseDTOs.Genres;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class GenresController : BaseCrudController<Genre, GenreService, GenreRequest, GenreResponse>
{
    public GenresController(GenreService service) : base(service)
    {
    }

    protected override Genre MapToEntity(GenreRequest request)
    {
        return new Genre
        {
            Name = request.Name
        };
    }
    
    // Maps Genre entity to GenreResponse DTO (for all responses)
    protected override GenreResponse MapToResponse(Genre entity)
    {
        return new GenreResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    // Updates existing Genre entity with values from GenreRequest (for Update)
    protected override void UpdateEntity(Genre entity, GenreRequest request)
    {
        entity.Name = request.Name;
    }
}
