using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Controllers
{
    [Authorize(Roles = "Cashier")]
    public class ProductController : ApiController
    {
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();
            return data.GetProducts();
        }
    }
}
