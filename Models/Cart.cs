using AzureAppINTEX.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureAppINTEX.Models
{
    public class Cart
    {
        public List<LineItem> Lines { get; set; } = new List<LineItem>();

        public virtual void AddItem(Product product, int quantity)
        {
            var line = Lines.FirstOrDefault(p => p.ProductID == product.ProductID);

            if (line == null)
            {
                Lines.Add(new LineItem
                {
                    ProductID = product.ProductID,
                    Product = product,
                    Quantity = quantity, // Assuming Quantity is treated as non-nullable in LineItem
                    Rating = 0 // Assuming a default value; adjust as necessary
                });
            }
            else
            {
                if (line.Quantity.HasValue) line.Quantity += quantity;
                else line.Quantity = quantity;
            }
        }

        public virtual void RemoveItem(int productId) =>
            Lines.RemoveAll(l => l.ProductID == productId);

        public virtual decimal ComputeTotalValue() =>
            Lines.Sum(e => e.Product.Price * (e.Quantity ?? 0));

        public virtual void Clear() => Lines.Clear();


    }
}
