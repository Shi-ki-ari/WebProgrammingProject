using System.Linq;
using API.Infrastructure.RequestDTOs.Movies;
using API.Infrastructure.ResponseDTOs.Movies;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class MoviesController : BaseCrudController<Movie, MovieService, MovieRequest, MovieResponse>
{
    public MoviesController(MovieService service) : base(service)
    {
    }

    protected override Movie MapToEntity(MovieRequest request)
    {
        return new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseYear = request.ReleaseYear
        };
    }

    protected override MovieResponse MapToResponse(Movie entity)
    {
        return new MovieResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            ReleaseYear = entity.ReleaseYear,
            Genres = entity.MovieGenres?.Select(mg => mg.Genre.Name).ToList() ?? new List<string>(),
            Actors = entity.MovieActors?.Select(ma => ma.Actor.Name).ToList() ?? new List<string>(),
            Languages = entity.MovieLanguages?.Select(ml => ml.Language.Name).ToList() ?? new List<string>(),
            ReviewCount = entity.Reviews?.Count ?? 0,
            AverageRating = entity.Reviews != null && entity.Reviews.Any() 
                ? entity.Reviews.Average(r => r.Rating) 
                : 0
        };
    }

    protected override void UpdateEntity(Movie entity, MovieRequest request)
    {
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ReleaseYear = request.ReleaseYear;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public override IActionResult GetAll()
    {
        var movies = Service.GetAllWithRelations();
        var response = movies.Select(m => MapToResponse(m)).ToList();
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public override IActionResult GetById(int id)
    {
        var movie = Service.GetByIdWithRelations(id);
        
        if (movie == null)
            return NotFound();
        
        return Ok(MapToResponse(movie));
    }
    [HttpPost]
    public override IActionResult Create([FromBody] MovieRequest request)
    {
        var movie = MapToEntity(request);
        var savedMovie = Service.CreateWithRelations(movie, request.GenreIds, request.ActorIds, request.LanguageIds);
        return Ok(MapToResponse(savedMovie));
    }

    [HttpPut("{id}")]
    public override IActionResult Update(int id, [FromBody] MovieRequest request)
    {
        var movie = Service.GetById(id);
        if (movie == null)
            return NotFound();

        UpdateEntity(movie, request);
        
        var updatedMovie = Service.UpdateWithRelations(movie, request.GenreIds, request.ActorIds, request.LanguageIds);
        return Ok(MapToResponse(updatedMovie));
    }
}

