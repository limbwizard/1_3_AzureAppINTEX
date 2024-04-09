using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAppINTEX.Models
{
    public class Order
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("Customer")]
        public string? CustomerID { get; set; }
        public DateTime? Date { get; set; }
        public string? DayOfWeek { get; set; }
        public int? Time { get; set; }
        public string? EntryMode { get; set; }
        public decimal Amount { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? CountryOfTransaction { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Bank { get; set; }
        public string? TypeOfCard { get; set; }
        public int? Fraud { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<LineItem>? LineItems { get; set; }
    }

}
