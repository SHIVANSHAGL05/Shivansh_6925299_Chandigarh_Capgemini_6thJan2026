using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace ProductClient.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ProductsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Product> Products { get; set; } = new();

        [BindProperty]
        public Product NewProduct { get; set; } = new();

        [BindProperty]
        public Product EditProduct { get; set; } = new();

        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadProductsAsync();
        }

        // CREATE
        public async Task<IActionResult> OnPostCreateAsync()
        {
            var client = _clientFactory.CreateClient("ProductApi");
            var content = new StringContent(
                JsonSerializer.Serialize(NewProduct),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("products", content);
            StatusMessage = response.IsSuccessStatusCode
                ? "Product created successfully!"
                : "Failed to create product.";

            await LoadProductsAsync();
            return Page();
        }

        // UPDATE
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var client = _clientFactory.CreateClient("ProductApi");
            var content = new StringContent(
                JsonSerializer.Serialize(EditProduct),
                Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"products/{EditProduct.Id}", content);
            StatusMessage = response.IsSuccessStatusCode
                ? "Product updated successfully!"
                : "Failed to update product.";

            await LoadProductsAsync();
            return Page();
        }

        // DELETE
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient("ProductApi");
            var response = await client.DeleteAsync($"products/{id}");
            StatusMessage = response.IsSuccessStatusCode
                ? "Product deleted."
                : "Failed to delete product.";

            await LoadProductsAsync();
            return Page();
        }

        private async Task LoadProductsAsync()
        {
            var client = _clientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync("products");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions) ?? new();
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
