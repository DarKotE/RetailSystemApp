using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public List<ProductModel> Get()
        {
            var data = new ProductData(_configuration);
            return data.GetProducts();
        }
    }
}
