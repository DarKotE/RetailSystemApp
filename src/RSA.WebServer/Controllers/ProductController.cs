using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSA.WebServer.Library.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private readonly IProductData _productData;

        public ProductController(IProductData productData)
        {
            _productData = productData;
        }
        [HttpGet]
        [Route("GetAllProducts")]
        public List<ProductModel> GetAllProducts()
        {
            return _productData.GetProducts();
        }
    }
}
