using ProductCatalog.Models;

namespace ProductCatalog.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> Search(string query, string? category = null, string? sort = null);
        IEnumerable<Product> GetFeatured();
        Product? GetById(int id);
        IEnumerable<string> GetCategories();
    }

    public class ProductService : IProductService
    {
        private static readonly List<Product> _products = new()
        {
            new Product { Id=1,  Name="Arc Wireless Headphones",    Category="Audio",       Price=129.99m, Rating=4.8, ReviewCount=342, IsNew=false, IsFeatured=true,
                Description="Premium over-ear headphones with 40-hour battery life, active noise cancellation, and Hi-Res Audio certification. Foldable design with memory-foam ear cushions.",
                ImageUrl="https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&q=80" },

            new Product { Id=2,  Name="Lumina Smart Watch",         Category="Wearables",   Price=249.00m, Rating=4.6, ReviewCount=215, IsNew=true,  IsFeatured=true,
                Description="Always-on AMOLED display, heart-rate & SpO2 monitoring, GPS, 5-day battery. Water-resistant to 50m. Pairs with iOS and Android seamlessly.",
                ImageUrl="https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600&q=80" },

            new Product { Id=3,  Name="Orion Mechanical Keyboard",  Category="Peripherals", Price=89.95m,  Rating=4.7, ReviewCount=189, IsNew=false, IsFeatured=false,
                Description="Compact TKL layout with hot-swappable switches, per-key RGB, and aircraft-grade aluminium frame. Tactile brown switches included.",
                ImageUrl="https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=600&q=80" },

            new Product { Id=4,  Name="Nova 4K Webcam",             Category="Peripherals", Price=74.99m,  Rating=4.5, ReviewCount=98,  IsNew=true,  IsFeatured=false,
                Description="Crystal-clear 4K 30fps video, built-in ring light, AI auto-framing, and dual noise-cancelling mics. Plug-and-play USB-C.",
                ImageUrl="https://images.unsplash.com/photo-1611532736597-de2d4265fba3?w=600&q=80" },

            new Product { Id=5,  Name="Vertex Portable Speaker",    Category="Audio",       Price=59.00m,  Rating=4.4, ReviewCount=276, IsNew=false, IsFeatured=true,
                Description="360° surround sound, IP67 waterproof, 20-hour playtime, and dual-pairing mode. Rugged fabric exterior fits any adventure.",
                ImageUrl="https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=600&q=80" },

            new Product { Id=6,  Name="Slate Pro Laptop Stand",     Category="Accessories", Price=49.99m,  Rating=4.9, ReviewCount=412, IsNew=false, IsFeatured=false,
                Description="Adjustable aluminium laptop stand with 6 height settings. Folds flat in seconds, weighs only 380g. Compatible with 10–17\" laptops.",
                ImageUrl="https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=600&q=80" },

            new Product { Id=7,  Name="Zenith USB-C Hub",           Category="Accessories", Price=39.99m,  Rating=4.3, ReviewCount=154, IsNew=false, IsFeatured=false,
                Description="7-in-1 hub: 100W PD charging, 4K HDMI, 3× USB-A 3.0, SD & microSD. Slim aluminium body stays cool under load.",
                ImageUrl="https://images.unsplash.com/photo-1625895197185-efcec01cffe0?w=600&q=80" },

            new Product { Id=8,  Name="Pulse Gaming Mouse",         Category="Peripherals", Price=64.50m,  Rating=4.6, ReviewCount=308, IsNew=true,  IsFeatured=true,
                Description="16,000 DPI optical sensor, 7 programmable buttons, ambidextrous design, and RGB underglow. Ultra-lightweight at 68g.",
                ImageUrl="https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=600&q=80" },

            new Product { Id=9,  Name="Eclipse Monitor Light",      Category="Accessories", Price=34.99m,  Rating=4.7, ReviewCount=227, IsNew=false, IsFeatured=false,
                Description="Screen-mounted LED bar with automatic brightness adjustment, 2700K–6500K colour temperature range, and zero glare design.",
                ImageUrl="https://images.unsplash.com/photo-1616400619175-5beda3a17896?w=600&q=80" },

            new Product { Id=10, Name="Aura True Wireless Earbuds", Category="Audio",       Price=99.00m,  Rating=4.5, ReviewCount=491, IsNew=true,  IsFeatured=true,
                Description="Active noise cancellation, 8h + 24h charging case, IPX5 splash-proof, transparency mode, and custom EQ via companion app.",
                ImageUrl="https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?w=600&q=80" },

            new Product { Id=11, Name="Phantom Desk Pad",           Category="Accessories", Price=24.99m,  Rating=4.8, ReviewCount=183, IsNew=false, IsFeatured=false,
                Description="Extra-large 900×400mm stitched-edge desk mat with non-slip rubber base, water-resistant micro-texture surface for mice and keyboards.",
                ImageUrl="https://images.unsplash.com/photo-1585790050230-5dd28404ccb9?w=600&q=80" },

            new Product { Id=12, Name="Volt 65W GaN Charger",       Category="Accessories", Price=44.95m,  Rating=4.6, ReviewCount=139, IsNew=true,  IsFeatured=false,
                Description="Compact 65W GaN charger with 2× USB-C and 1× USB-A. Charges a MacBook Pro or three devices simultaneously. Foldable prongs.",
                ImageUrl="https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?w=600&q=80" },
        };

        public IEnumerable<Product> GetAll() => _products;

        public IEnumerable<Product> Search(string query, string? category = null, string? sort = null)
        {
            var result = _products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                result = result.Where(p =>
                    p.Name.ToLower().Contains(q) ||
                    p.Description.ToLower().Contains(q) ||
                    p.Category.ToLower().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
                result = result.Where(p => p.Category == category);

            result = sort switch
            {
                "price_asc"  => result.OrderBy(p => p.Price),
                "price_desc" => result.OrderByDescending(p => p.Price),
                "rating"     => result.OrderByDescending(p => p.Rating),
                "newest"     => result.OrderByDescending(p => p.IsNew),
                _            => result.OrderBy(p => p.Name)
            };

            return result;
        }

        public IEnumerable<Product> GetFeatured() =>
            _products.Where(p => p.IsFeatured).Take(4);

        public Product? GetById(int id) =>
            _products.FirstOrDefault(p => p.Id == id);

        public IEnumerable<string> GetCategories() =>
            _products.Select(p => p.Category).Distinct().OrderBy(c => c);
    }
}
