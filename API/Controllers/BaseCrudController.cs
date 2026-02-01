using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Base controller providing CRUD operations for all entities
// TEntity: The entity type (Genre, Language, etc.)
// TService: The service type (GenreService, LanguageService, etc.)
// TRequest: The request DTO type (GenreRequest, LanguageRequest, etc.)
// TResponse: The response DTO type (GenreResponse, LanguageResponse, etc.)
[ApiController]
[Route("api/[controller]")]
public abstract class BaseCrudController<TEntity, TService, TRequest, TResponse> : ControllerBase
    where TEntity : BaseEntity, new()
    where TService : BaseService<TEntity>, new()
{
    // Service instance shared by all child controllers
    protected TService Service = new TService();
    
    // Child controllers must implement these mapping methods
    protected abstract TEntity MapToEntity(TRequest request);
    protected abstract TResponse MapToResponse(TEntity entity);
    protected abstract void UpdateEntity(TEntity entity, TRequest request);
    
    // GET: api/{controller}
    // Returns all items from the database
    [HttpGet]
    public virtual IActionResult GetAll()
    {
        var entities = Service.GetAll();
        var response = entities.Select(e => MapToResponse(e)).ToList();
        return Ok(response);
    }
    
    // GET: api/{controller}/5
    // Returns a single item by ID
    [HttpGet("{id}")]
    public virtual IActionResult GetById(int id)
    {
        var entity = Service.GetById(id);
        
        if (entity == null)
            return NotFound();
        
        return Ok(MapToResponse(entity));
    }
    
    // POST: api/{controller}
    // Creates a new item
    [HttpPost]
    public virtual IActionResult Create([FromBody] TRequest request)
    {
        var entity = MapToEntity(request);
        Service.Save(entity);
        return Ok(MapToResponse(entity));
    }
    
    // PUT: api/{controller}/5
    // Updates an existing item
    [HttpPut("{id}")]
    public virtual IActionResult Update(int id, [FromBody] TRequest request)
    {
        var entity = Service.GetById(id);
        
        if (entity == null)
            return NotFound();
        
        UpdateEntity(entity, request);
        Service.Save(entity);
        return Ok(MapToResponse(entity));
    }
    
    // DELETE: api/{controller}/5
    // Deletes an item by ID
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var entity = Service.GetById(id);
        
        if (entity == null)
            return NotFound();
        
        Service.Delete(entity);
        return Ok();
    }
}
