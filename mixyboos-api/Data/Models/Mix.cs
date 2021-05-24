using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MixyBoos.Api.Data.Models {
    public class Mix {
        public int Id { get; set; }
        public MixyBoosUser User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool Active { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Inserted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Updated { get; set; }
    }
}
