using ECommerceOrderAPI.Data;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceOrderAPI.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            log.Info("Product fetch request received - fetching all products");

            try
            {
                var products = _context.Products.ToList();
                log.Info($"Products fetched successfully | Count: {products.Count}");
                return Ok(products);
            }
            catch (Exception ex)
            {
                log.Error("Exception while fetching all products", ex);
                return StatusCode(500, new { message = "Error fetching products." });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            log.Info($"Product fetch request received for ProductId: {id}");

            try
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    log.Warn($"Product not found for ProductId: {id}");
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                log.Info($"Product fetched successfully | ProductId: {id} | Name: {product.Name}");
                return Ok(product);
            }
            catch (Exception ex)
            {
                log.Error($"Exception while fetching ProductId: {id}", ex);
                return StatusCode(500, new { message = "Error fetching product." });
            }
        }
    }
}
