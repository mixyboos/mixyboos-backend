using System;
using System.Collections.Generic;

namespace MixyBoos.Api.Data.Models;

public class LiveShow : BaseEntity {
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsFinished { get; set; } = false;
    public virtual MixyBoosUser User { get; set; }

    public ICollection<Tag> Tags { get; set; }
}
