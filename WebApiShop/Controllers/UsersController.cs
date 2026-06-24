using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Metrics;
using System.Text.Json;
using Services;
using Entities;
using System.Threading.Tasks;
using DTOs;
using WebApiShop.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            this._usersService = usersService;
        }

        // GET: api/<UsersController>
        [AuthorizeRole(Roles.Admin)]
        [HttpGet]
        public async Task<IEnumerable<UserDTO>> Get()
        {

            return await _usersService.GetUsers();
        }

        // GET api/<UsersController>/5
        [Authorize]
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] UserDTO user)
        {
            if(! _usersService.IsPasswordStrong(user.Password))
                return BadRequest("Please insert strong password");
            if(! await _usersService.UserWithSameEmail(user.UserName, user.UserId))
                return BadRequest("Email is already in use");
            AuthResultDTO? result =  await _usersService.CreateUser(user);
            if (result == null)
                return BadRequest("Something went wrong, please try again");
            SetTokenCookie(result.Token);
            return CreatedAtAction(nameof(Get), new { id = result.User.UserId }, result.User);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Post1([FromBody] LoginUserDTO loggedUser)
        {
            AuthResultDTO? result = await _usersService.Login(loggedUser);
            if (result != null)
            {
                SetTokenCookie(result.Token);
                return CreatedAtAction(nameof(Get), new { result.User.UserId }, result.User);
            }
            return NoContent();
        }

        // PUT api/<UsersController>/5
        [Authorize]
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

        private void SetTokenCookie(string token)
        {
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }
    }
}
