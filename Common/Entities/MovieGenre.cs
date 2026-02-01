using System;

namespace Common.Entities;

public class MovieGenre
{
    public int MovieId { get; set; }
    public int GenreId { get; set; }
    
    // Navigation properties
    public virtual Movie Movie { get; set; }
    public virtual Genre Genre { get; set; }
}
