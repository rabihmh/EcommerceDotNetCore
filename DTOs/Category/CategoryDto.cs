namespace EcommerceDotNetCore.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; } = null;

}