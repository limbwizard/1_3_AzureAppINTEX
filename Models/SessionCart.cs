using Microsoft.AspNetCore.Http;
using AzureAppINTEX.Infrastructure;
using System.Text.Json.Serialization;
using AzureAppINTEX.Models;

namespace AzureAppINTEX.Models
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession Session { get; private set; }

        // Factory method to retrieve Cart from session or create new.
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

        public override void RemoveItem(int productId)
        {
            base.RemoveItem(productId);
            Session.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session.Remove("Cart");
        }
    }
}
