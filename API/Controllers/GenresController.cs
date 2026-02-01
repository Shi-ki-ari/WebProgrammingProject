using System.Linq;
using API.Infrastructure.RequestDTOs.Genres;
using API.Infrastructure.ResponseDTOs.Genres;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
// Sets the route pattern - [controller] is replaced with "Genres", so route becomes "api/genres"
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    // Service instance for database operations
    private GenreService _genreService;

    // Constructor - creates a new GenreService instance when controller is created
    public GenresController()
    {
        _genreService = new GenreService();
    }

    // GET: api/genres
    // Returns all genres from the database
    [HttpGet]
    public IActionResult GetAll()
    {
        // 1. Get all genre entities from database using service
        var genres = _genreService.GetAll();

        // 2. Map each Genre entity to GenreResponse DTO
        // This converts database entities to API response format
        var response = genres.Select(g => new GenreResponse
        {
            Id = g.Id,
            Name = g.Name
        }).ToList();

        // 3. Return HTTP 200 OK with the list of genres
        return Ok(response);
    }

    // GET: api/genres/5
    // Returns a single genre by ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // 1. Get genre entity from database by ID
        var genre = _genreService.GetById(id);

        // 2. Check if genre exists - if not, return HTTP 404 Not Found
        if (genre == null)
            return NotFound();

        // 3. Map Genre entity to GenreResponse DTO
        var response = new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };

        // 4. Return HTTP 200 OK with the genre data
        return Ok(response);
    }

    // POST: api/genres
    // Creates a new genre
    [HttpPost]
    public IActionResult Create([FromBody] GenreRequest request)
    {
        // 1. Map GenreRequest DTO to Genre entity
        // Note: Id is not set - database will auto-generate it
        var genre = new Genre
        {
            Name = request.Name
        };

        // 2. Save the new genre to database
        // BaseService.Save detects Id=0 and performs INSERT
        _genreService.Save(genre);

        // 3. Map the saved entity (now with Id) to GenreResponse DTO
        var response = new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };

        // 4. Return HTTP 200 OK with the created genre
        return Ok(response);
    }

    // PUT: api/genres/5
    // Updates an existing genre
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] GenreRequest request)
    {
        // 1. Find the existing genre in the database
        var genre = _genreService.GetById(id);

        // 2. Check if genre exists - if not, return HTTP 404 Not Found
        if (genre == null)
            return NotFound();

        // 3. Update the genre's properties with new values from request
        genre.Name = request.Name;

        // 4. Save the updated genre to database
        // BaseService.Save detects Id>0 and performs UPDATE
        _genreService.Save(genre);

        // 5. Map the updated entity to GenreResponse DTO
        var response = new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };

        // 6. Return HTTP 200 OK with the updated genre
        return Ok(response);
    }

    // DELETE: api/genres/5
    // Deletes a genre by ID
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // 1. Find the genre to delete
        var genre = _genreService.GetById(id);

        // 2. Check if genre exists - if not, return HTTP 404 Not Found
        if (genre == null)
            return NotFound();

        // 3. Delete the genre from database
        _genreService.Delete(genre);

        // 4. Return HTTP 200 OK (no content needed for delete)
        return Ok();
    }
}
