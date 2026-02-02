using System.Linq;
using API.Infrastructure.RequestDTOs.Movies;
using API.Infrastructure.ResponseDTOs.Movies;
using Common.Entities;
using Common.Persistence;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class MoviesController : BaseCrudController<Movie, MovieService, MovieRequest, MovieResponse>
{
    private readonly AppDbContext _context;

    public MoviesController(MovieService service, AppDbContext context) : base(service)
    {
        _context = context;
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
    
    // Override GetAll to use eager loading (public - no auth required)
    [HttpGet]
    public override IActionResult GetAll()
    {
        var movies = _context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .ToList();
        var response = movies.Select(m => MapToResponse(m)).ToList();
        return Ok(response);
    }
    
    // Override GetById to use eager loading (public - no auth required)
    [HttpGet("{id}")]
    public override IActionResult GetById(int id)
    {
        var movie = _context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .FirstOrDefault(m => m.Id == id);
        
        if (movie == null)
            return NotFound();
        
        return Ok(MapToResponse(movie));
    }

    [HttpPost]
    public override IActionResult Create([FromBody] MovieRequest request)
    {
        var movie = MapToEntity(request);
        _context.Movies.Add(movie);
        _context.SaveChanges();

        if (request.GenreIds != null)
        {
            foreach (var genreId in request.GenreIds)
            {
                _context.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId
                });
            }
        }

        if (request.ActorIds != null)
        {
            foreach (var actorId in request.ActorIds)
            {
                _context.MovieActors.Add(new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                });
            }
        }

        if (request.LanguageIds != null)
        {
            foreach (var languageId in request.LanguageIds)
            {
                _context.MovieLanguages.Add(new MovieLanguage
                {
                    MovieId = movie.Id,
                    LanguageId = languageId
                });
            }
        }

        _context.SaveChanges();
        
        var savedMovie = _context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .FirstOrDefault(m => m.Id == movie.Id);
        return Ok(MapToResponse(savedMovie));
    }

    [HttpPut("{id}")]
    public override IActionResult Update(int id, [FromBody] MovieRequest request)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
            return NotFound();

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.ReleaseYear = request.ReleaseYear;

        var existingGenres = _context.MovieGenres.Where(mg => mg.MovieId == movie.Id).ToList();
        _context.MovieGenres.RemoveRange(existingGenres);

        var existingActors = _context.MovieActors.Where(ma => ma.MovieId == movie.Id).ToList();
        _context.MovieActors.RemoveRange(existingActors);

        var existingLanguages = _context.MovieLanguages.Where(ml => ml.MovieId == movie.Id).ToList();
        _context.MovieLanguages.RemoveRange(existingLanguages);

        if (request.GenreIds != null)
        {
            foreach (var genreId in request.GenreIds)
            {
                _context.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId
                });
            }
        }

        if (request.ActorIds != null)
        {
            foreach (var actorId in request.ActorIds)
            {
                _context.MovieActors.Add(new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                });
            }
        }

        if (request.LanguageIds != null)
        {
            foreach (var languageId in request.LanguageIds)
            {
                _context.MovieLanguages.Add(new MovieLanguage
                {
                    MovieId = movie.Id,
                    LanguageId = languageId
                });
            }
        }

        _context.SaveChanges();
        
        var updatedMovie = _context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .FirstOrDefault(m => m.Id == movie.Id);
        return Ok(MapToResponse(updatedMovie));
    }
}

