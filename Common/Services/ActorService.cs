using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class ActorService : BaseService<Actor>
{
    public ActorService(AppDbContext context) : base(context)
    {
    }
}
