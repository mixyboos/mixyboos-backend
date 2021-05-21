#nullable disable

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MixyBoos.Api {
    public class BaseEntity {
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    }

    public class Mix : BaseEntity {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
