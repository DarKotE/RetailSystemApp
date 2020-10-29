using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class InventoryData
    {
        private readonly IConfiguration _configuration;

        public InventoryData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<InventoryModel> GetInventory()
        {
            var sql = new SqlDataAccess(_configuration);
            return sql
                    .LoadData<InventoryModel, dynamic>(storedProcedure: "dbo.spInventory_GetAll",
                                                        parameters: new { },
                                                        connectionStringName: "RSAData");
        }
        public void SaveInventoryRecord(InventoryModel inventoryRecord)
        {
            var sql = new SqlDataAccess(_configuration);
            sql.SaveData(storedProcedure: "[dbo].[spInventory_Insert]",
                         parameters: inventoryRecord,
                         connectionStringName: "RSAData");
        }
    }
}
