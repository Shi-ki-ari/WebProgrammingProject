using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class MovieService : BaseService<Movie>
{
    public MovieService(AppDbContext context) : base(context)
    {
    }
}
