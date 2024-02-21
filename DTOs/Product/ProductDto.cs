
using System.ComponentModel.DataAnnotations;

namespace EcommerceDotNetCore.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }
        
        public int CategoryId { get; set; }
        [Required]
        public IFormFile Image { get; set; }

    }
}