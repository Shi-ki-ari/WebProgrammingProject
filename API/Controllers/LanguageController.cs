using API.Infrastructure.RequestDTOs.Languages;
using API.Infrastructure.ResponseDTOs.Languages;
using Common.Entities;
using Common.Services;

namespace API.Controllers;

// LanguageController inherits all CRUD operations from BaseCrudController
// Only needs to implement the mapping logic between DTOs and entities
public class LanguageController : BaseCrudController<Language, LanguageService, LanguageRequest, LanguageResponse>
{
    // Maps LanguageRequest DTO to Language entity (for Create)
    protected override Language MapToEntity(LanguageRequest request)
    {
        return new Language
        {
            Name = request.Name
        };
    }
    
    // Maps Language entity to LanguageResponse DTO (for all responses)
    protected override LanguageResponse MapToResponse(Language entity)
    {
        return new LanguageResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    // Updates existing Language entity with values from LanguageRequest (for Update)
    protected override void UpdateEntity(Language entity, LanguageRequest request)
    {
        entity.Name = request.Name;
    }
}