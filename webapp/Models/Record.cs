using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace webapp.Models {

    public class Record {

        [Key]
        public long RecordId { get; set; }

        public string? Note { get; set; }

        public string? Category { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsDone { get; set; } = false;

        [NotMapped]
        public string Status => IsDone ? "Closed" : "Pending";

        [NotMapped]
        public string FormattedAmount => Amount.ToString("C2", CultureInfo.CurrentCulture);
    }
}
