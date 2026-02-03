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
    var genres = new List<string>();
    if (entity.MovieGenres != null)
    {
        genres = entity.MovieGenres.Select(mg => mg.Genre.Name).ToList();
    }

    var actors = new List<string>();
    if (entity.MovieActors != null)
    {
        actors = entity.MovieActors.Select(ma => ma.Actor.Name).ToList();
    }

    var languages = new List<string>();
    if (entity.MovieLanguages != null)
    {
        languages = entity.MovieLanguages.Select(ml => ml.Language.Name).ToList();
    }

    int reviewCount = 0;
    double averageRating = 0;

    if (entity.Reviews != null)
    {
        reviewCount = entity.Reviews.Count;

        if (entity.Reviews.Count > 0)
        {
            averageRating = entity.Reviews.Average(r => r.Rating);
        }
    }

    return new MovieResponse
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        ReleaseYear = entity.ReleaseYear,
        Genres = genres,
        Actors = actors,
        Languages = languages,
        ReviewCount = reviewCount,
        AverageRating = averageRating
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

