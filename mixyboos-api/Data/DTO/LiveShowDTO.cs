using System;
using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO {
    public class LiveShowDTO {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public ProfileDTO User { get; set; }
        public List<string> Tags { get; set; }
    }
}
