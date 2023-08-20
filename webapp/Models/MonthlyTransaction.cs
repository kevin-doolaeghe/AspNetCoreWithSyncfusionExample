using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace webapp.Models {

    public class MonthlyTransaction {

        [Key]
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public Category? Category { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

        public string? Note { get; set; }

        public double Amount { get; set; }

        public int DayOfMonth { get; set; }

        [NotMapped]
        public string FormattedAmount => Amount.ToString("C2", CultureInfo.CurrentCulture);
    }
}
