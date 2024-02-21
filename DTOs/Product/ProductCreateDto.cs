using System.ComponentModel.DataAnnotations;

namespace EcommerceDotNetCore.DTOs.Product
{
    public class ProductCreateDto
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.Now;
       [Required]
        public IFormFile Image { get; set; }
    }
}
