using System;
using System.Collections.Generic;

namespace Common.Entities;

public class Actor : BaseEntity
{
    public string Name { get; set; }
    public string Bio { get; set; }
    
    public virtual List<MovieActor> MovieActors { get; set; }
}
