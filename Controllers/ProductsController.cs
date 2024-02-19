using AutoMapper;
using EcommerceDotNetCore.DTOs.Product;
using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDotNetCore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController:ControllerBase
{
    private readonly IRepository<Product> _productRepository;
    private IMapper _mapper;

    public ProductsController(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
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
    public async Task<IActionResult> CreateProduct(ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var product = _mapper.Map<Product>(productDto);
        await _productRepository.AddAsync(product);
        return StatusCode(201);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
        var product = await _productRepository.FindByIdAsync(id);
        if (product == null||productDto.Id!=id)
        {
            return NotFound();
        }
        _mapper.Map(productDto, product);
        await _productRepository.UpdateAsync(product);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.FindByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        await _productRepository.DeleteAsync(product);
        return Ok();
    }
    
}