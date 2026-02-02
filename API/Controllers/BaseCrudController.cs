using System.Linq;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseCrudController<TEntity, TService, TRequest, TResponse> : ControllerBase
    where TEntity : BaseEntity, new()
    where TService : BaseService<TEntity>
{
    protected readonly TService Service;
    
    protected BaseCrudController(TService service)
    {
        Service = service;
    }
    
    protected abstract TEntity MapToEntity(TRequest request);
    protected abstract TResponse MapToResponse(TEntity entity);
    protected abstract void UpdateEntity(TEntity entity, TRequest request);
    




//crud operations
    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult GetAll()
    {
        var entities = Service.GetAll();
        var response = entities.Select(MapToResponse).ToList();
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public virtual IActionResult GetById(int id)
    {
        var entity = Service.GetById(id);
        
        if (entity == null)
            return NotFound();
        
        return Ok(MapToResponse(entity));
    }
    
    [HttpPost]
    public virtual IActionResult Create([FromBody] TRequest request)
    {
        var entity = MapToEntity(request);
        Service.Save(entity);
        return Ok(MapToResponse(entity));
    }
    
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
    
    [HttpDelete("{id}")]
    public virtual IActionResult Delete(int id)
    {
        var entity = Service.GetById(id);
        
        if (entity == null)
            return NotFound();
        
        Service.Delete(entity);
        return Ok();
    }
}
