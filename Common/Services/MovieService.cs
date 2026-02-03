using Common.Entities;
using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Common.Services;

public class MovieService : BaseService<Movie>
{
    public MovieService(AppDbContext context) : base(context)
    {
    }

    public List<Movie> GetAllWithRelations()
    {
        return Context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .ToList();
    }

    public Movie? GetByIdWithRelations(int id)
    {
        return Context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieLanguages).ThenInclude(ml => ml.Language)
            .Include(m => m.Reviews)
            .FirstOrDefault(m => m.Id == id);
    }

        public Movie CreateWithRelations(Movie movie, List<int>? genreIds, List<int>? actorIds, List<int>? languageIds)
        {
            Context.Movies.Add(movie);
            Context.SaveChanges();

            //add genres
            if (genreIds != null)
            {
                foreach (var genreId in genreIds)
                {
                    Context.MovieGenres.Add(new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genreId
                    });
                }
            }

            //add actors
            if (actorIds != null)
            {
                foreach (var actorId in actorIds)
                {
                    if (actorId > 0)
                    {
                        Context.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }
                }
            }

            //add languages
            if (languageIds != null)
            {
                foreach (var languageId in languageIds)
                {
                    if (languageId > 0)
                    {
                        Context.MovieLanguages.Add(new MovieLanguage
                        {
                            MovieId = movie.Id,
                            LanguageId = languageId
                        });
                    }
                }
            }

            Context.SaveChanges();
            return GetByIdWithRelations(movie.Id)!;
        }

        public Movie UpdateWithRelations(Movie movie, List<int>? genreIds, List<int>? actorIds, List<int>? languageIds)
        {
            //delete old relationships
            var existingGenres = Context.MovieGenres.Where(mg => mg.MovieId == movie.Id).ToList();
            foreach (var genre in existingGenres)
            {
                Context.MovieGenres.Remove(genre);
            }

            var existingActors = Context.MovieActors.Where(ma => ma.MovieId == movie.Id).ToList();
            foreach (var actor in existingActors)
            {
                Context.MovieActors.Remove(actor);
            }

            var existingLanguages = Context.MovieLanguages.Where(ml => ml.MovieId == movie.Id).ToList();
            foreach (var language in existingLanguages)
            {
                Context.MovieLanguages.Remove(language);
            }

            //make new ones
            if (genreIds != null)
            {
                foreach (var genreId in genreIds)
                {
                    Context.MovieGenres.Add(new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genreId
                    });
                }
            }

            if (actorIds != null)
            {
                foreach (var actorId in actorIds)
                {
                    if (actorId > 0)
                    {
                        Context.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }
                }
            }

            if (languageIds != null)
            {
                foreach (var languageId in languageIds)
                {
                    if (languageId > 0)
                    {
                        Context.MovieLanguages.Add(new MovieLanguage
                        {
                            MovieId = movie.Id,
                            LanguageId = languageId
                        });
                    }
                }
            }

            Context.SaveChanges();
            return GetByIdWithRelations(movie.Id);
        }
    }
