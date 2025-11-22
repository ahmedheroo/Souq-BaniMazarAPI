using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Souq_BaniMazarAPI.Data;
using Souq_BaniMazarAPI.DTOs;
using Souq_BaniMazarAPI.Models;
using System.Threading.Tasks;

namespace Souq_BaniMazarAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        public ProductsController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            var products = _dbcontext.Products.Include(p => p.Categories).Include(p => p.Seller).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoriesId,
                CategoryName = p.Categories.Name,
                ImgUrl = p.ImgUrl,
                Quantity = p.Quantity,
                SellerId = p.SellerId,
                SellerName = p.Seller != null ? p.Seller.Name : null
            }).ToList();
            return Ok(products);
        }
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _dbcontext.Products.Include(p => p.Categories).Include(p => p.Seller).Where(p => p.Id == id).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoriesId,
                CategoryName = p.Categories.Name,
                ImgUrl = p.ImgUrl,
                Quantity = p.Quantity,
                SellerId = p.SellerId,
                SellerName = p.Seller != null ? p.Seller.Name : null
            }).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto product)
        {
            try
            {
            var newProduct = new Products
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoriesId = product.CategoryId,
                Quantity = product.Quantity,
                StatusId=product.StatusId,
                SellerId = product.SellerId
            };
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Products", $"{newProduct.SellerId}");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(product.ProductImg.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                   await product.ProductImg.CopyToAsync(stream);
                }
                newProduct.ImgUrl = $"/uploads/Products/{newProduct.SellerId}/{fileName}";
                _dbcontext.Products.Add(newProduct);
                _dbcontext.SaveChanges();
                return Ok(new { Message = "Product added successfully", ProductId = newProduct.Id });
            }
            catch (Exception ex)
            {   
                return StatusCode(500, "An error occurred while adding the product."+ex);
            } 
        }

    }
}
