using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
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
