using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Services;
using Entities;
using System.Threading.Tasks;
using DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ICategoriesService _categoriesService;
        public CategoriesController(ICategoriesService categoriesService)
        {
            this._categoriesService = categoriesService;
        }

        // GET: api/<CategorysController>
        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> Get()
        {
            
            return await _categoriesService.GetCategories();
        }

        // POST api/<CategorysController>
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Post([FromBody] CategoryDTO category)
        {
            CategoryDTO? _category =  await _categoriesService.CreateCategory(category);
            if (_category == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, category);
        }

    }
}
