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

public class MoviesController : BaseCrudController<Movie, MovieService, MovieRequest, MovieResponse>
{
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
    [AllowAnonymous]
    public override IActionResult GetAll()
    {
        using (var context = new AppDbContext())
        {
            var movies = context.Movies
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
                .Include(m => m.Reviews)
                .ToList();
            var response = movies.Select(m => MapToResponse(m)).ToList();
            return Ok(response);
        }
    }
    
    // Override GetById to use eager loading (public - no auth required)
    [HttpGet("{id}")]
    [AllowAnonymous]
    public override IActionResult GetById(int id)
    {
        using (var context = new AppDbContext())
        {
            var movie = context.Movies
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
                .Include(m => m.Reviews)
                .FirstOrDefault(m => m.Id == id);
            
            if (movie == null)
                return NotFound();
            
            return Ok(MapToResponse(movie));
        }
    }
    //this is where crud ends and dbcontext operations begin

    [Authorize]
    public override IActionResult Create([FromBody] MovieRequest request)
    {
        // Use the same context for everything
        using (var context = new AppDbContext())
        {
            // 1. Create and save the movie entity
            var movie = MapToEntity(request);
            context.Movies.Add(movie);
            context.SaveChanges(); // Save to get the generated Id

            // 2. Create junction table records
            if (request.GenreIds != null)
            {
                foreach (var genreId in request.GenreIds)
                {
                    context.MovieGenres.Add(new MovieGenre
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
                    context.MovieActors.Add(new MovieActor
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
                    context.MovieLanguages.Add(new MovieLanguage
                    {
                        MovieId = movie.Id,
                        LanguageId = languageId
                    });
                }
            }

            context.SaveChanges();
            
            // Reload with eager loading to get navigation properties
            var savedMovie = context.Movies
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
                .Include(m => m.Reviews)
                .FirstOrDefault(m => m.Id == movie.Id);
            return Ok(MapToResponse(savedMovie));
        }
    }

    [Authorize]
    public override IActionResult Update(int id, [FromBody] MovieRequest request)
    {
        using (var context = new AppDbContext())
        {
            // 1. Get existing movie
            var movie = context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();

            // 2. Update movie properties
            movie.Title = request.Title;
            movie.Description = request.Description;
            movie.ReleaseYear = request.ReleaseYear;

            // 3. Clear old relationships
            var existingGenres = context.MovieGenres.Where(mg => mg.MovieId == movie.Id).ToList();
            context.MovieGenres.RemoveRange(existingGenres);

            var existingActors = context.MovieActors.Where(ma => ma.MovieId == movie.Id).ToList();
            context.MovieActors.RemoveRange(existingActors);

            var existingLanguages = context.MovieLanguages.Where(ml => ml.MovieId == movie.Id).ToList();
            context.MovieLanguages.RemoveRange(existingLanguages);

            // 4. Add new relationships
            if (request.GenreIds != null)
            {
                foreach (var genreId in request.GenreIds)
                {
                    context.MovieGenres.Add(new MovieGenre
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
                    context.MovieActors.Add(new MovieActor
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
                    context.MovieLanguages.Add(new MovieLanguage
                    {
                        MovieId = movie.Id,
                        LanguageId = languageId
                    });
                }
            }

            context.SaveChanges();
            
            // Reload with eager loading to get navigation properties
            var updatedMovie = context.Movies
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
                .Include(m => m.Reviews)
                .FirstOrDefault(m => m.Id == movie.Id);
            return Ok(MapToResponse(updatedMovie));
        }
    }
}

