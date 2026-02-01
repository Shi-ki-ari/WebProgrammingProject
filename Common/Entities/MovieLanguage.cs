using System;

namespace Common.Entities;

public class MovieLanguage : BaseEntity
{
    public int MovieId { get; set; }
    public int LanguageId { get; set; }
    
    // Navigation properties
    public virtual Movie Movie { get; set; }
    public virtual Language Language { get; set; }
}
