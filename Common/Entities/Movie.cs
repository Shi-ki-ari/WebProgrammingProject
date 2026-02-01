using System;
using System.Collections.Generic;

namespace Common.Entities;

public class Movie : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    
    // Navigation properties
    public virtual List<MovieGenre> MovieGenres { get; set; }
    public virtual List<MovieActor> MovieActors { get; set; }
    public virtual List<MovieLanguage> MovieLanguages { get; set; }
    public virtual List<Review> Reviews { get; set; }
}
