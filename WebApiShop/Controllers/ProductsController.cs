using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Services;
using Entities;
using System.Threading.Tasks;
using DTOs;
using StackExchange.Redis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IProductsService _productsService;
        private readonly IDatabase _db;
        private readonly IConfiguration _config;
        public ProductsController(IProductsService productsService, IConnectionMultiplexer redis, IConfiguration config)
        {
            this._productsService = productsService;
            this._db = redis.GetDatabase();
            this._config = config;
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<PageResponseDTO> Get([FromQuery] int[] categoryId, [FromQuery] decimal maxPrice, [FromQuery] decimal minPrice, [FromQuery] int position = 1, [FromQuery] int skip = 20, [FromQuery] string desc = "")
        {
            try
            {
                string cacheKey = Request.Path + Request.QueryString;

                var cachedData = await _db.StringGetAsync(cacheKey);
                if (!cachedData.IsNull)
                {
                    return JsonSerializer.Deserialize<PageResponseDTO>(cachedData);
                }
                var products = await _productsService.GetProducts(categoryId, maxPrice, minPrice, desc, position, skip);
                var ttlMinutes = _config.GetValue<int>("Redis:DefaultTTLInMinutes", 30);
                await _db.StringSetAsync(cacheKey, JsonSerializer.Serialize(products), TimeSpan.FromMinutes(ttlMinutes));
                return products;
            }
            catch (Exception ex)
            {
                return await _productsService.GetProducts(categoryId, maxPrice, minPrice, desc, position, skip);
            }
            
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            try 
            {
                string cacheKey = Request.Path;
                var cachedData = await _db.StringGetAsync(cacheKey);
                if (!cachedData.IsNull)
                {
                    return JsonSerializer.Deserialize<ProductDTO>(cachedData);
                }
                ProductDTO? product = await _productsService.GetProductById(id);
                var ttlMinutes = _config.GetValue<int>("Redis:DefaultTTLInMinutes", 30);
                await _db.StringSetAsync(cacheKey, JsonSerializer.Serialize(product), TimeSpan.FromMinutes(ttlMinutes));
                return Ok(product);
            }
            catch (Exception e)
            {
                return await _productsService.GetProductById(id);
            }


        }
        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO product)
        {
            ProductDTO? _product =  await _productsService.CreateProduct(product);
            if (_product == null)
                return BadRequest();
            try
            {
                await ClearProductsCache();
            }
            catch (Exception ex) { }
            return CreatedAtAction(nameof(Get), new { id = _product.ProductId }, _product);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProductDTO product)
        {
            try 
            {
                await _productsService.UpdateProduct(id, product);
                try 
                {
                    await _db.KeyDeleteAsync($"{Request.Path}");
                    await ClearProductsCache();
                }
                catch(Exception ex) { }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task ClearProductsCache()
        {
            // 1. מקבלים את נקודות הקצה (EndPoints) של השרת
            var endpoints = _db.Multiplexer.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                // 2. מקבלים גישה לשרת הספציפי
                var server = _db.Multiplexer.GetServer(endpoint);

                // 3. סורקים את המפתחות שמתחילים בנתיב של המוצרים
                // שימוש ב-KeysAsync עדיף על Keys כי הוא משתמש ב-SCAN מאחורי הקלעים ולא חוסם את השרת
                var keys = server.Keys(pattern: "/api/Products*");

                foreach (var key in keys)
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }
    }
}
