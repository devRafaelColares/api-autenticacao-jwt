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
        [AllowAnonymous]
        public IActionResult AddUser([FromBody] User user)
        {
            User userCreated = _repository.Add(user);

            var token = _tokenGenerator.GenerateToken(userCreated);

            return Created("", new { message = "User created and logged in successfully", token });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTORequest loginDTO)
        {
            User? existingUser = _repository.GetUserByEmail(loginDTO.Email!);
            if (existingUser == null || existingUser.Password != loginDTO.Password)
                return Unauthorized(new { message = "Incorrect e-mail or password" });

            var token = _tokenGenerator.GenerateToken(existingUser);

            return Ok(new { message = "Login successful", token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAll()
        {
            var users = _repository.GetAll();
            if (users == null || !users.Any())
            {
                return NotFound(new { message = "No users found" });
            }
            return Ok(users);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = _repository.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;
            user.Name = updatedUser.Name;
            user.Access = updatedUser.Access;

            _repository.Update(user);

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            var user = _repository.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _repository.Delete(id);

            return Ok(new { message = "User deleted successfully" });
        }
    }
}