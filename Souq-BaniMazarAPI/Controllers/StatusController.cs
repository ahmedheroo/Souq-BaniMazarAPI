using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Souq_BaniMazarAPI.Data;
using Souq_BaniMazarAPI.Models;

namespace Souq_BaniMazarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;
        public StatusController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpGet("GetAllStatuses")]
        public IActionResult GetAllStatuses()
        {
            var statusList = new List<ProductStatus>();
            statusList = _dbcontext.ProductStatus.ToList();
            return Ok(statusList);
        }
    }
}
