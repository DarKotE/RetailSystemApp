using System.Collections.Generic;
using System.Linq;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sql;

        public ProductData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public List<ProductModel> GetProducts()
        {
            return _sql
                .LoadData<ProductModel, dynamic>(storedProcedure: "[dbo].[spProduct_GetAll]",
                                                 parameters: new { },
                                                 connectionStringName: "RSAData");
        }
        public ProductModel GetProductById(int productId)
        {
            return _sql
                .LoadData<ProductModel, dynamic>(storedProcedure: "[dbo].[spProduct_GetById]",
                                                 parameters: new { Id = productId },
                                                 connectionStringName: "RSAData").FirstOrDefault();
        }
    }
}
