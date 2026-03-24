using System.ComponentModel.DataAnnotations;

namespace ECommerceOMS.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        [Range(0, 1000)]
        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();

        [Display(Name = "Product Count")]
        public int ProductCount => Products?.Count ?? 0;
    }
}
