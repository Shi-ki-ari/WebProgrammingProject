using API.Infrastructure.RequestDTOs.Languages;
using API.Infrastructure.ResponseDTOs.Languages;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class LanguageController : BaseCrudController<Language, LanguageService, LanguageRequest, LanguageResponse>
{
    public LanguageController(LanguageService service) : base(service)
    {
    }

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