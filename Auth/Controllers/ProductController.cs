using Microsoft.AspNetCore.Mvc;
using Auth.Models;
using Auth.Repository;
using Auth.DTO;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new { message = "Rota autorizada [AllowAnonymous]" });
        }

        [HttpPost]
        [Authorize(Policy = "levelA")]
        public IActionResult Post()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var name = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var email = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            return Created("", new { message = "Rota protegida por level A", name, email });
        }

        [HttpDelete]
        [Authorize(Policy = "levelB")]
        public IActionResult Delete()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var name = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var email = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            
            return Ok(new { message = "Rota protegida por level B", name, email });
        }
    }
}
