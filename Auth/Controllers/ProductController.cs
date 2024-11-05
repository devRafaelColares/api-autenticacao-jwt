using Microsoft.AspNetCore.Mvc;
using Auth.Models;
using Auth.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth.Controllers
{
    [ApiController]
    [Route("product")]
    [Authorize] // Exigir autenticação para todas as rotas neste controlador
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous] // Permitir acesso anônimo para consulta de produtos
        public IActionResult Get()
        {
            var products = _repository.GetAll();
            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No products found" });
            }
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Policy = "levelA")]
        public IActionResult Post([FromBody] Product product)
        {
            _repository.Add(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, new { message = "Product created successfully", product });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "levelB")]
        public IActionResult Put(int id, [FromBody] Product updatedProduct)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            product.Model = updatedProduct.Model;
            product.Brand = updatedProduct.Brand;
            product.Price = updatedProduct.Price;
            product.ManufactureDate = updatedProduct.ManufactureDate;

            _repository.Update(product);

            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "levelB")]
        public IActionResult Delete(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            _repository.Delete(id);

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}