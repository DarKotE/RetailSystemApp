using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;

namespace RSAWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        public List<ProductModel> Get()
        {
            var data = new ProductData();
            return data.GetProducts();
        }
    }
}
