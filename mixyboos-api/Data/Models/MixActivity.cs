﻿#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace MixyBoos.Api.Data.Models;

public class MixActivity : BaseEntity {
  public Guid MixId { get; set; }
  public virtual Mix Mix { get; set; }

  public Guid? UserId { get; set; }
  public virtual MixyBoosUser? User { get; set; }
}

public class MixPlay : MixActivity { }

public class MixLike : MixActivity { }

public class MixShare : MixActivity { }

public class MixDownload : MixActivity { }
