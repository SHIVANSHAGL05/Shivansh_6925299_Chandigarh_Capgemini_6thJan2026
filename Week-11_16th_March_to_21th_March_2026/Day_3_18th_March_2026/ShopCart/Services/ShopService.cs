using ShopCart.Models;

namespace ShopCart.Services
{
    public class ShopService
    {
        // ── Seed Products ────────────────────────────────────────
        private static readonly List<Product> _products = new()
        {
            new Product { Id=1,  Name="Wireless Headphones",  Category="Electronics", Price=1299, Description="Over-ear headphones with noise cancellation and 30hr battery.", ImageUrl="https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&q=80" },
            new Product { Id=2,  Name="Smart Watch",          Category="Electronics", Price=2499, Description="Fitness tracker with heart-rate monitor and sleep tracking.",  ImageUrl="https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400&q=80" },
            new Product { Id=3,  Name="Running Shoes",        Category="Footwear",    Price=1799, Description="Lightweight running shoes with cushioned sole and mesh upper.", ImageUrl="https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400&q=80" },
            new Product { Id=4,  Name="Cotton T-Shirt",       Category="Clothing",    Price=399,  Description="Premium 100% cotton t-shirt available in multiple colours.",   ImageUrl="https://images.unsplash.com/photo-1527719327859-c6ce80353573?w=400&q=80" },
            new Product { Id=5,  Name="Leather Backpack",     Category="Bags",        Price=1599, Description="Genuine leather backpack with laptop compartment, 30L.",       ImageUrl="https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400&q=80" },
            new Product { Id=6,  Name="Bluetooth Speaker",    Category="Electronics", Price=899,  Description="Portable waterproof speaker with 360° surround sound.",        ImageUrl="https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=400&q=80" },
            new Product { Id=7,  Name="Stainless Water Bottle",Category="Accessories",Price=349,  Description="Insulated 750ml bottle keeps drinks cold for 24 hours.",       ImageUrl="https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=400&q=80" },
            new Product { Id=8,  Name="Yoga Mat",             Category="Sports",      Price=599,  Description="Non-slip 6mm yoga mat with carry strap, eco-friendly.",        ImageUrl="https://images.unsplash.com/photo-1601925228058-a1b9cff7fc2b?w=400&q=80" },
        };

        // ── Cart (per session, stored in service) ────────────────
        // In a real app this would use ISession or a database.
        // Here we use a simple static dictionary keyed by session ID.
        private static readonly Dictionary<string, List<CartItem>> _carts = new();

        // ── Orders ───────────────────────────────────────────────
        private static readonly List<Order> _orders = new();
        private static int _nextOrderId = 1;

        // Products
        public List<Product> GetAllProducts() => _products;
        public List<Product> GetByCategory(string category) =>
            _products.Where(p => p.Category == category).ToList();
        public Product? GetProduct(int id) =>
            _products.FirstOrDefault(p => p.Id == id);
        public List<string> GetCategories() =>
            _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

        // Cart helpers
        public List<CartItem> GetCart(string sessionId)
        {
            if (!_carts.ContainsKey(sessionId))
                _carts[sessionId] = new List<CartItem>();
            return _carts[sessionId];
        }

        public void AddToCart(string sessionId, int productId)
        {
            var cart    = GetCart(sessionId);
            var product = GetProduct(productId);
            if (product == null) return;

            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null)
                existing.Quantity++;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name      = product.Name,
                    Price     = product.Price,
                    Quantity  = 1,
                    ImageUrl  = product.ImageUrl
                });
        }

        public void UpdateQuantity(string sessionId, int productId, int quantity)
        {
            var cart = GetCart(sessionId);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return;

            if (quantity <= 0)
                cart.Remove(item);
            else
                item.Quantity = quantity;
        }

        public void RemoveFromCart(string sessionId, int productId)
        {
            var cart = GetCart(sessionId);
            cart.RemoveAll(c => c.ProductId == productId);
        }

        public void ClearCart(string sessionId)
        {
            if (_carts.ContainsKey(sessionId))
                _carts[sessionId].Clear();
        }

        public int CartCount(string sessionId) =>
            GetCart(sessionId).Sum(c => c.Quantity);

        public decimal CartTotal(string sessionId) =>
            GetCart(sessionId).Sum(c => c.Subtotal);

        // Orders
        public Order PlaceOrder(string sessionId, Order orderDetails)
        {
            var cart = GetCart(sessionId);
            orderDetails.Id       = _nextOrderId++;
            orderDetails.OrderRef = "ORD-" + orderDetails.Id.ToString("D4");
            orderDetails.PlacedOn = DateTime.Now;
            orderDetails.Total    = CartTotal(sessionId);
            orderDetails.Items    = new List<CartItem>(cart);

            _orders.Add(orderDetails);
            ClearCart(sessionId);
            return orderDetails;
        }

        public Order? GetOrder(int id) =>
            _orders.FirstOrDefault(o => o.Id == id);
    }
}
