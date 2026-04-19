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
    public class ProductsController : ControllerBase
    {
        readonly IProductsService _productsService;
        public ProductsController(IProductsService productsService)
        {
            this._productsService = productsService;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<PageResponseDTO> Get([FromQuery] int[] categoryId, [FromQuery] decimal maxPrice, [FromQuery] decimal minPrice, [FromQuery] int position = 1, [FromQuery] int skip = 20, [FromQuery] string desc = "")
        {
            
            return await _productsService.GetProducts(categoryId, maxPrice, minPrice, desc, position, skip);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            ProductDTO? Product = await _productsService.GetProductById(id);
            return Ok(Product);
            
        }

    }
}
