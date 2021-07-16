using System;

namespace MixyBoos.Api.Data.Models {
    public class LiveShow : BaseEntity {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public bool Active { get; set; }
        public virtual MixyBoosUser User { get; set; }
    }
}
