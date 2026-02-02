using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class GenreService : BaseService<Genre>
{
    public GenreService(AppDbContext context) : base(context)
    {
    }
}
