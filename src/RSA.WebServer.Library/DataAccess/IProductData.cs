using System.Collections.Generic;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public interface IProductData
    {
        List<ProductModel> GetProducts();
        ProductModel GetProductById(int productId);
    }
}