using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAppINTEX.Models
{
    public class LineItem
    {
        [Key]
        public int LineItemID { get; set; }
        [ForeignKey("Order")]
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public int? Quantity { get; set; }
        public int? Rating { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }

}
