using Common.Entities;
using Common.Persistence;
using System.Linq;

namespace Common.Services;

public class UserService : BaseService<User>
{
    public UserService(AppDbContext context) : base(context)
    {
    }

    public User FindByUsername(string username)
    {
        return Context.Users.FirstOrDefault(u => u.Username == username);
    }

    public User FindByUsernameOrEmail(string username, string email)
    {
        return Context.Users
            .FirstOrDefault(u => u.Username == username || u.Email == email);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }
}
