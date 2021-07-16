using System;

namespace MixyBoos.Api.Data.Utils {
    public class SlugFieldAttribute : Attribute {
        public string SourceField { get; set; }
    }
}
