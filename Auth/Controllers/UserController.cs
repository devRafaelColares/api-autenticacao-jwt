using Microsoft.AspNetCore.Mvc;
using Auth.Models;
using Auth.Repository;
using Auth.DTO;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Auth.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly TokenGenerator _tokenGenerator;

        public UserController(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _tokenGenerator = new TokenGenerator(configuration);
        }

        [HttpPost("signup")]
        public IActionResult AddUser([FromBody] User user)
        {
            User userCreated = _repository.Add(user);

            var token = _tokenGenerator.GenerateToken(userCreated);

            // Definir o cookie JWT com HttpOnly e Secure
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // Somente para HTTPS em produção
                SameSite = SameSiteMode.Strict, // Proteção contra CSRF
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Created("", new { message = "User created and logged in successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTORequest loginDTO)
        {
            User? existingUser = _repository.GetUserByEmail(loginDTO.Email!);
            if (existingUser == null || existingUser.Password != loginDTO.Password)
                return Unauthorized(new { message = "Incorrect e-mail or password" });

            var token = _tokenGenerator.GenerateToken(existingUser);

            // Configurar o cookie JWT
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // Somente para HTTPS em produção
                SameSite = SameSiteMode.Strict, // Proteção contra CSRF
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Remover o cookie JWT
            Response.Cookies.Delete("jwt_token");

            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }
    }
}
