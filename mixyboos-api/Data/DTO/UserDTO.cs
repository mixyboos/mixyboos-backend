using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO;

/// <summary>
/// This is the PUBLIC profile that can be safely returned to anyone.
/// </summary>
public record UserDTO {
  public string Title { get; set; }
  public string ProfileImage { get; set; }
  public string HeaderImage { get; set; }
  public string Slug { get; set; }
  public string DisplayName { get; set; }
  public string City { get; set; }
  public string Country { get; set; }
  public string Biography { get; set; }
  public string PhoneNumber { get; set; }

  public List<FollowDTO> Followers { get; set; }
  public List<FollowDTO> Following { get; set; }
}
