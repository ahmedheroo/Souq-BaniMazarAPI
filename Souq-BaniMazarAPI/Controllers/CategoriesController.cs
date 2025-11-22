using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Souq_BaniMazarAPI.Data;
using Souq_BaniMazarAPI.DTOs;
using Souq_BaniMazarAPI.Models;

namespace Souq_BaniMazarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        public CategoriesController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpGet("GetCategoriesWithProducts")]
        public IActionResult GetCategoriesWithProducts()
        {
            var categories = _dbcontext.Categories.Where(p=>p.Products.Count()>0).AsNoTracking().Select(c=> new CategoriesWithProductsDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                ProductsCount=c.Products.Count(),
                Products =c.Products.Select(p=> new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    ImgUrl = p.ImgUrl,
                    SellerName=p.Seller.Name,
                    SellerId=p.Seller.Id
                }).ToList()
            }).ToList();
            return Ok(categories);
        }
        [HttpGet("GetCategoriesWithProductsCount")]
        public IActionResult GetCategoriesWithProductsCount()
        {
            var categories = _dbcontext.Categories.AsNoTracking().Where(p=>p.Products.Count()!=0).Include(c => c.Products).Select(s =>
            new CategoriesWithProductsCountDto
            {
                CategoryId = s.Id,
                CategoryName = s.Name,
                ImgUrl=s.ImgUrl,
                ProductsCount = s.Products.Count
            }).Take(7).ToList();

            return Ok(categories);
        }
        [HttpGet("GetCategoriesList")]
        public IActionResult GetCategoriesList()
        {
            var categories = _dbcontext.Categories.Select(s =>
            new Categories
            {
                Id = s.Id,
                Name = s.Name,
            }).ToList();

            return Ok(categories);
        }
    }
}
