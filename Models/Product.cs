using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceDotNetCore.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }
    public string ImagePath { get; set; }
    [ForeignKey("Categories")]
    public int CategoryId { get; set; }
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public virtual Category? Category { get; set; }

}