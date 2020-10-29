using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration _configuration;

        public ProductData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<ProductModel> GetProducts()
        {
            var sqlAccess = new SqlDataAccess(_configuration);
            return sqlAccess
                .LoadData<ProductModel, dynamic>(storedProcedure: "[dbo].[spProduct_GetAll]",
                                                 parameters: new { },
                                                 connectionStringName: "RSAData");
        }
        public ProductModel GetProductById(int productId)
        {
            var sqlAccess = new SqlDataAccess(_configuration);
            return sqlAccess
                .LoadData<ProductModel, dynamic>(storedProcedure: "[dbo].[spProduct_GetById]",
                                                 parameters: new { Id = productId },
                                                 connectionStringName: "RSAData").FirstOrDefault();
        }
    }
}
