using Microsoft.AspNetCore.Http;
using AzureAppINTEX.Infrastructure;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace AzureAppINTEX.Models
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession Session { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        public override void AddItem(Product product, int quantity)
        {
            base.AddItem(product, quantity);
            Session.SetJson("Cart", this);
        }

        // Updated to accept ProductID for removal
        public void RemoveItem(int productId)
        {
            base.RemoveItem(productId); // Adjust base method to accept ProductID
            Session.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session.Remove("Cart");
        }
    }
}
