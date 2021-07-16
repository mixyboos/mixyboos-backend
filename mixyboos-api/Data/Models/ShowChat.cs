#nullable enable
using System;

namespace MixyBoos.Api.Data.Models {
    public class ShowChat : BaseEntity {
        public MixyBoosUser FromUser { get; set; }
        public MixyBoosUser? ToUser { get; set; }
        public DateTime DateSent { get; set; }

        public virtual LiveShow Show { get; set; }
    }
}
