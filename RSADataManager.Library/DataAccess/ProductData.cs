using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
{
    public class ProductData
    {
        public List<ProductModel> GetProducts() 
        {
            var sqlAccess = new SqlDataAccess();
            return sqlAccess
                .LoadData<ProductModel, dynamic>(storedProcedure: "[dbo].[spProduct_GetAll]",
                                                 parameters: new { },
                                                 connectionStringName: "RSAData");
        }
    }
}
