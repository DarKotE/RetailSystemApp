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
