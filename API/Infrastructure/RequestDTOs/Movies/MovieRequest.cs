using System.Collections.Generic;

namespace API.Infrastructure.RequestDTOs.Movies;

public class MovieRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    public List<int> GenreIds { get; set; }
    public List<int> ActorIds { get; set; }
    public List<int> LanguageIds { get; set; }
}
