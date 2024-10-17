using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO {
    public class DebugDTO {
        public string LibVersion { get; set; }
        public string OSVersion { get; set; }
        public Dictionary<string, string> Config { get; set; }
    }
}
