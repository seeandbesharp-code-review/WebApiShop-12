using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        readonly IPasswordsService _passwordsService;

        public PasswordsController(IPasswordsService passwordsService)
        {
            this._passwordsService = passwordsService;
        }

        // POST api/<PasswordController>
        [HttpPost]
        public ActionResult<int> Post([FromBody] string value)
        {
            int level = _passwordsService.passwordValidation(value);
            return Ok(level);

        }
    }
}
