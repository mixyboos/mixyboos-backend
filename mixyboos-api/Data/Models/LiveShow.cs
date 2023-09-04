using System;
using System.Collections.Generic;

namespace MixyBoos.Api.Data.Models;

public enum ShowStatus {
  Checking,
  Setup,
  AwaitingStreamConnection,
  InProgress,
  Ended,
  Error,
}

public class LiveShow : BaseEntity {
  public bool IsFinished => Status.Equals(ShowStatus.Ended);
  public string Title { get; set; }
  public string Description { get; set; }
  public DateTime StartDate { get; set; }
  public ShowStatus Status { get; set; } = ShowStatus.Setup;
  public virtual MixyBoosUser User { get; set; }
  public ICollection<Tag> Tags { get; set; }
}
