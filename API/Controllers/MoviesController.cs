using System.Linq;
using API.Infrastructure.RequestDTOs.Movies;
using API.Infrastructure.ResponseDTOs.Movies;
using Common.Entities;
using Common.Persistence;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// MoviesController handles CRUD for movies and their relationships
// Overrides Create and Update to manage junction tables (MovieGenre, MovieActor, MovieLanguage)
public class MoviesController : BaseCrudController<Movie, MovieService, MovieRequest, MovieResponse>
{
    // DbContext for direct access to junction tables
    private AppDbContext _context = new AppDbContext();

    // Maps MovieRequest DTO to Movie entity (for Create)
    // Only handles simple properties - relationships handled separately
    protected override Movie MapToEntity(MovieRequest request)
    {
        return new Movie
        {
            Title = request.Title,
            Description = request.Description,
            ReleaseYear = request.ReleaseYear
            // GenreIds, ActorIds, LanguageIds are handled in Create/Update methods
        };
    }

    // Maps Movie entity to MovieResponse DTO (for all responses)
    // Includes related data: genre names, actor names, language names, review stats
    protected override MovieResponse MapToResponse(Movie entity)
    {
        return new MovieResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            ReleaseYear = entity.ReleaseYear,
            // Extract genre names from navigation properties
            Genres = entity.MovieGenres?.Select(mg => mg.Genre.Name).ToList() ?? new System.Collections.Generic.List<string>(),
            // Extract actor names from navigation properties
            Actors = entity.MovieActors?.Select(ma => ma.Actor.Name).ToList() ?? new System.Collections.Generic.List<string>(),
            // Extract language names from navigation properties
            Languages = entity.MovieLanguages?.Select(ml => ml.Language.Name).ToList() ?? new System.Collections.Generic.List<string>(),
            // Count total reviews
            ReviewCount = entity.Reviews?.Count ?? 0,
            // Calculate average rating (0 if no reviews)
            AverageRating = entity.Reviews != null && entity.Reviews.Any() 
                ? entity.Reviews.Average(r => r.Rating) 
                : 0
        };
    }

    // Updates existing Movie entity with values from MovieRequest (for Update)
    // Clears old junction table records - new ones will be created in Update method
    protected override void UpdateEntity(Movie entity, MovieRequest request)
    {
        // Update simple properties
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ReleaseYear = request.ReleaseYear;

        // Clear existing relationships using DbContext directly
        var existingGenres = _context.MovieGenres.Where(mg => mg.MovieId == entity.Id).ToList();
        _context.MovieGenres.RemoveRange(existingGenres);

        var existingActors = _context.MovieActors.Where(ma => ma.MovieId == entity.Id).ToList();
        _context.MovieActors.RemoveRange(existingActors);

        var existingLanguages = _context.MovieLanguages.Where(ml => ml.MovieId == entity.Id).ToList();
        _context.MovieLanguages.RemoveRange(existingLanguages);

        _context.SaveChanges();
    }

    // Override Create to handle junction tables after saving the movie
    [HttpPost]
    public new IActionResult Create([FromBody] MovieRequest request)
    {
        // 1. Create and save the movie entity (gets Id from database)
        var movie = MapToEntity(request);
        Service.Save(movie);

        // 2. Create MovieGenre junction records
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

        // 3. Create MovieActor junction records
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

        // 4. Create MovieLanguage junction records
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

        // 5. Return the complete movie with all relationships
        return Ok(MapToResponse(movie));
    }

    // Override Update to handle junction tables after updating the movie
    [HttpPut("{id}")]
    public new IActionResult Update(int id, [FromBody] MovieRequest request)
    {
        // 1. Get existing movie
        var movie = Service.GetById(id);
        if (movie == null)
            return NotFound();

        // 2. Update movie properties and clear old relationships
        UpdateEntity(movie, request);
        Service.Save(movie);

        // 3. Create new MovieGenre junction records
        if (request.GenreIds != null)
        {
            foreach (var genreId in request.GenreIds)
            {
                var movieGenre = new MovieGenre();
                _context.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId
                });
            }
        }

        // 4. Create new MovieActor junction records
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

        // 5. Create new MovieLanguage junction records
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
        // 6. Return the updated movie with all relationships
        return Ok(MapToResponse(movie));
    }
}
