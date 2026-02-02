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
    public class OrdersController : ControllerBase
    {
        IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            this._ordersService = ordersService;
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<IEnumerable<OrderDTO>> Get()
        {
            
            return await _ordersService.GetOrders();
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> Get(int id)
        {
            OrderDTO order = await _ordersService.GetOrderById(id);
            
                return Ok(order);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> Post([FromBody] OrderDTO order)
        {
            OrderDTO _order =  await _ordersService.CreateOrder(order);
            if (_order == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = order.OrderId }, order);
        }

        
    }
}
