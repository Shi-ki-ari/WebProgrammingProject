using Common.Entities;
using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Common.Services;

public class ReviewService : BaseService<Review>
{
    public ReviewService(AppDbContext context) : base(context)
    {
    }

    public List<Review> GetAllWithMovieAndUser()
    {
        return Context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .ToList();
    }

    public Review GetByIdWithMovieAndUser(int id)
    {
        return Context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == id);
    }

    public Review GetExistingReview(int userId, int movieId)
    {
        return Context.Reviews
            .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);
    }
}
