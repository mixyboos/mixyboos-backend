using System;

namespace MixyBoos.Api.Data {
    public class SlugFieldAttribute : Attribute {
        public string SourceField { get; set; }
    }
}
