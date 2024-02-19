using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceDotNetCore.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [ForeignKey("ParentCategory")]
    public int? ParentCategoryId { get; set; } = null;
    [JsonIgnore]
    public virtual Category ParentCategory { get; set; }

    public virtual ICollection<Category> ChildCategories { get; set; }
    public virtual ICollection<Product> Products { get; set; }

}