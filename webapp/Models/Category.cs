using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.Models {

    public class Category {

        [Key]
        public long CategoryId { get; set; }

        public string? Icon { get; set; }

        public string? Name { get; set; }

        [NotMapped]
        public string Description => $"{Icon} {Name}";
    }
}
