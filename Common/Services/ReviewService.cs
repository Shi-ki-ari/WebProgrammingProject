using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class ReviewService : BaseService<Review>
{
    public ReviewService(AppDbContext context) : base(context)
    {
    }
}
