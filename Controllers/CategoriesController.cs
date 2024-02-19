using AutoMapper;
using EcommerceDotNetCore.DTOs.Category;
using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
       private readonly IRepository<Category> _categoryRepository;
       private readonly IMapper _mapper;


        public CategoriesController(IRepository<Category> categoryRepository,IMapper mapper)
       {
           _categoryRepository = categoryRepository;
           _mapper = mapper;
       }

       [HttpGet]
       public async Task<ActionResult<IEnumerable<Category>>> Get()
       {
           var categories = await _categoryRepository.GetAllAsync();
           return Ok(categories);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categoryDto=_mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.AddAsync(category);
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category is null ||categoryDto.Id!=id )
            {
                return NotFound();
            }

            _mapper.Map(categoryDto, category);
            try
            {
                await _categoryRepository.UpdateAsync(category);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteAsync(category);

            return NoContent();
        }


    }
}
