using System;
using System.Collections.Generic;

namespace Common.Entities;

public class Language : BaseEntity
{
    public string Name { get; set; }
    
    public virtual List<MovieLanguage> MovieLanguages { get; set; }
}
