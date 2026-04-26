namespace ProductApi.DTOs;

public class CreateProductRequestDto
{
    public required string Name { get; set; }

    public decimal Price { get; set; }

    public IFormFile? File { get; set; }
}
