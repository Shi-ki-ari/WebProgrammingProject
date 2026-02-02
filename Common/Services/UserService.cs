using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class UserService : BaseService<User>
{
    public UserService(AppDbContext context) : base(context)
    {
    }
}
