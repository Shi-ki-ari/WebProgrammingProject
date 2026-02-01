using System;
using System.Collections.Generic;

namespace Common.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; }
    
    // Navigation properties
    public virtual List<MovieGenre> MovieGenres { get; set; }
}
