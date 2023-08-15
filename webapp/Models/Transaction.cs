using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace webapp.Models {

    public class Transaction {

        [Key]
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public Category? Category { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

        public string? Note { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public bool IsDone { get; set; } = false;

        [NotMapped]
        public string FormattedAmount => Amount.ToString("C2", CultureInfo.CurrentCulture);
    }
}
