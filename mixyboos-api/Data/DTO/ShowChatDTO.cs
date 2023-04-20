using System;

namespace MixyBoos.Api.Data.DTO;

public class ShowChatDTO {
    public string Id { get; set; }
    public ProfileDTO FromUser { get; set; }
    public ProfileDTO ToUser { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
}
