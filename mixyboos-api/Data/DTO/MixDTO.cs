using System;

namespace MixyBoos.Api.Data.DTO;

public class MixDTO {
  public string Id { get; set; }
  public string Slug { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public DateTime DateUploaded { get; set; }

  public string Image { get; set; }
  public bool IsProcessed { get; set; }
  public ProfileDTO User { get; set; }
  public int LikeCount { get; set; }
  public int PlayCount { get; set; }
  public int ShareCount { get; set; }
  public int DownloadCount { get; set; }
  public long Duration { get; set; }

  public string AudioUrl { get; set; }
  public string PcmUrl { get; set; }

  public string[] Tags { get; set; }
}
