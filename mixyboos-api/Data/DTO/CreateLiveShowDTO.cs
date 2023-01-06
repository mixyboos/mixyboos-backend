using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO; 

public class CreateLiveShowDTO {
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Tags { get; set; }
}
