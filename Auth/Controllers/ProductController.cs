using Microsoft.AspNetCore.Mvc;
using Auth.Models;
using Auth.Repository;
using Auth.DTO;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Auth.Controllers;

[ApiController]
[Route("product")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new { message = "Rota autorizada" });
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "levelA")]
    public IActionResult Post()
    {
        var token = HttpContext.User.Identity as ClaimsIdentity;
        var name = token?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var email = token?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        return Created("", new { message = "Rota protegida por level A", name, email });
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "levelB")]
    public IActionResult Delete()
    {
        var token = HttpContext.User.Identity as ClaimsIdentity;
        var name = token?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var email = token?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        
        return Ok(new { message = "Rota protegida por level B", name, email });
    }


}