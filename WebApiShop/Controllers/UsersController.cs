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
    public class UsersController : ControllerBase
    {
        IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            this._usersService = usersService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserDTO>> Get()
        {
            
            return await _usersService.GetUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            UserDTO? user = await _usersService.GetUserById(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NoContent();
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] UserDTO user)
        {
            UserDTO? _user =  await _usersService.CreateUser(user);
            if (_user == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = user.UserId }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Post1([FromBody] LoginUserDTO loggedUser)
        {
            UserDTO? user = await _usersService.Login(loggedUser);
            if (user != null)
                return CreatedAtAction(nameof(Get), new { user.UserId }, user);
            return NoContent();
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UserDTO user)
        {
            try 
            {
                await _usersService.UpdateUser(id, user);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
