using System.ComponentModel.DataAnnotations;

namespace AzureAppINTEX.Models
{
    public class LineItem
    {
        [Key]
        public int LineItemID { get; set; }
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public int? Quantity { get; set; }
        public int? Rating { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }

}
