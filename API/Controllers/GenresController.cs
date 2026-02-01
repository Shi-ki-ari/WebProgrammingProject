using API.Infrastructure.RequestDTOs.Genres;
using API.Infrastructure.ResponseDTOs.Genres;
using Common.Entities;
using Common.Services;

namespace API.Controllers;

// GenresController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class GenresController : BaseCrudController<Genre, GenreService, GenreRequest, GenreResponse>
{
    // Maps GenreRequest DTO to Genre entity (for Create)
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
