using Common.Entities;
using Common.Persistence;

namespace Common.Services;

public class LanguageService : BaseService<Language>
{
    public LanguageService(AppDbContext context) : base(context)
    {
    }
}
