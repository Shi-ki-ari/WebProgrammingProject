using System;

namespace Common.Entities;

public class Review : BaseEntity
{
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime DatePosted { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; }
    public virtual Movie Movie { get; set; }
}
