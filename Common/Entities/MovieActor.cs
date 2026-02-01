using System;

namespace Common.Entities;

public class MovieActor
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }
    
    // Navigation properties
    public virtual Movie Movie { get; set; }
    public virtual Actor Actor { get; set; }
}
