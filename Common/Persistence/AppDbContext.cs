using System;
using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Review> Reviews { get; set; }

    
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieLanguage> MovieLanguages { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region User

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@movie.com",
                Password = "parola",
                Role = "Admin"
            });
            
        #endregion

        #region Movie

        modelBuilder.Entity<Movie>()
            .HasKey(m => m.Id);

        #endregion

        #region Actor

        modelBuilder.Entity<Actor>()
            .HasKey(a => a.Id);

        #endregion

        #region Genre

        modelBuilder.Entity<Genre>()
            .HasKey(g => g.Id);

        #endregion

        #region Language

        modelBuilder.Entity<Language>()
            .HasKey(l => l.Id);

        #endregion

        #region Review

        modelBuilder.Entity<Review>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Movie)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade); 

        #endregion

        #region MovieGenre

        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });

        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieGenre>()
            .HasOne(mg => mg.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region MovieActor

        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region MovieLanguage

        modelBuilder.Entity<MovieLanguage>()
            .HasKey(ml => new { ml.MovieId, ml.LanguageId });

        modelBuilder.Entity<MovieLanguage>()
            .HasOne(ml => ml.Movie)
            .WithMany(m => m.MovieLanguages)
            .HasForeignKey(ml => ml.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieLanguage>()
            .HasOne(ml => ml.Language)
            .WithMany(l => l.MovieLanguages)
            .HasForeignKey(ml => ml.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}
