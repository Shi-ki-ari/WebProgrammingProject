using System.Collections.Generic;

namespace API.Infrastructure.ResponseDTOs.Movies;

public class MovieResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }

    public List<string> Genres { get; set; }
    public List<string> Actors { get; set; }
    public List<string> Languages { get; set; }
    
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
}
