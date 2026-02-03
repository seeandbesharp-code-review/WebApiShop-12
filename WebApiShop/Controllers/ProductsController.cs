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
        private readonly IProductsService _productsService;
        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IEnumerable<ProductDTO>> Get([FromQuery] int[]? categoryId, [FromQuery] decimal maxPrice, [FromQuery] decimal minPrice)
        {
            
            return await _productsService.GetProducts(categoryId, maxPrice, minPrice);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            ProductDTO? product = await _productsService.GetProductById(id);
            return Ok(product);
            
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO product)
        {
            ProductDTO? createdProduct =  await _productsService.CreateProduct(product);
            if (createdProduct == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = product.ProductId }, product);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO product)
        {
            try 
            {
                await _productsService.UpdateProduct(id, product);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        
    }
}
