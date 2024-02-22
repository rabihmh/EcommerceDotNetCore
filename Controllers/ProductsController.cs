using AutoMapper;
using EcommerceDotNetCore.DTOs.Product;
using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Repository;
using EcommerceDotNetCore.Services.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDotNetCore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController:ControllerBase
{
    private readonly IRepository<Product> _productRepository;
    private readonly IImageService _imageService;
    private IMapper _mapper;

    public ProductsController(IRepository<Product> productRepository, IMapper mapper, IImageService imageService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _imageService = imageService;   
    }
    [HttpGet]
    public async Task<IEnumerable<Product>> Get()
    {
        var products = await _productRepository.GetAllAsync();
        return products;
    }
    [HttpGet]
    [Route("{id}")]
    public async Task<Product> GetProductById(int id)
    {
        var product = await _productRepository.FindByIdAsync(id);
        return product;
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            string imagePath = null;

            if (productDto.Image != null)
            {
                imagePath = _imageService.UploadImage("wwwroot", "products", productDto.Image.FileName, productDto.Image.OpenReadStream());
            }

            var product = _mapper.Map<Product>(productDto);
            product.ImagePath = imagePath;

            await _productRepository.AddAsync(product);
                
            return StatusCode(201); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto)
    {
        var product = await _productRepository.FindByIdAsync(id);
        if (product == null || productDto.Id != id)
        {
            return NotFound();
        }

        try
        {
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                _imageService.DeleteImage("wwwroot", "products", Path.GetFileName(product.ImagePath));
            }

            string imagePath = product.ImagePath;

            if (productDto.Image != null)
            {
               imagePath= _imageService.UploadImage("wwwroot", "products", productDto.Image.FileName, productDto.Image.OpenReadStream());
            }
            product.ImagePath = imagePath;
            _mapper.Map(productDto, product);
            await _productRepository.UpdateAsync(product);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.FindByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        if (!string.IsNullOrEmpty(product.ImagePath))
        {
            _imageService.DeleteImage("wwwroot", "products", Path.GetFileName(product.ImagePath));
        }
        await _productRepository.DeleteAsync(product);
        return NoContent();
    }
    
}