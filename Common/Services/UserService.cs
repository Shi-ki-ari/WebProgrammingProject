using Common.Entities;
using Common.Persistence;
using System.Linq;

namespace Common.Services;

public class UserService : BaseService<User>
{
    public UserService(AppDbContext context) : base(context)
    {
    }

    public User FindByCredentials(string username, string password)
    {
        return Context.Users
            .FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public User FindByUsernameOrEmail(string username, string email)
    {
        return Context.Users
            .FirstOrDefault(u => u.Username == username || u.Email == email);
    }
}
